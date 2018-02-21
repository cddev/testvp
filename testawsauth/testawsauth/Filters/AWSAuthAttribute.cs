using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using testawsauth.Models;

namespace testawsauth.Filters
{
    public class AWSAuthAttribute : Attribute, IAuthenticationFilter
    {
        #region vars
        private readonly UInt64 requestMaxAge = 300; // in Seconds

        #endregion

        #region props


        public static IUserRepository _repository { get; set; }

        private string AuthSchema { get; set; }
        public bool AllowMultiple
        {
            get { return false; }
        }
        public AWSAuthAttribute()
        {
            AuthSchema = "foo";
        }
        #endregion

        #region  wrk

        #region public
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;

            if (request.Headers.Authorization != null && AuthSchema.Equals(request.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                string authHeader = request.Headers.Authorization.Parameter;

                string[] authHeaderArray = GetAuthHeaderValues(authHeader);

                if (authHeaderArray != null)
                {
                    var APPId = authHeaderArray[0];
                    var incomingBase64Signature = authHeaderArray[1];

                    var requestTimeStamp = authHeaderArray[2];

                    var isValid = IsValidRequest(request, APPId, incomingBase64Signature, requestTimeStamp);

                    if (isValid.Result)
                    {
                        var currentPrincipal = new GenericPrincipal(new GenericIdentity(APPId), null);
                        context.Principal = currentPrincipal;
                    }
                    else
                    {
                        context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                    }
                }
                else
                {
                    context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
                }
            }
            else
            {
                context.ErrorResult = new UnauthorizedResult(new AuthenticationHeaderValue[0], context.Request);
            }

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new RequestResult(context.Result);
            return Task.FromResult(0);
        }
        #endregion
        #region private 
        private async Task<bool> IsValidRequest(HttpRequestMessage req, string APPId, string incomingBase64Signature, string requestTimeStamp)
        {
            string requestContentBase64String = "";
            string requestUri = HttpUtility.UrlEncode(req.RequestUri.AbsoluteUri.ToLower());
            string requestHttpMethod = req.Method.Method;


            //Checking if AppId Exist
            if (!IsExisting(APPId))
            {
                return false;
            }




            //Checking if request is Time Out
            if (IsTimeOut(requestTimeStamp))
            {
                return false;
            }

            byte[] hash = await CreateMD5Hash(req.Content);

            if (hash != null)
            {
                requestContentBase64String = Convert.ToBase64String(hash);
            }

            //building signature with secret matching appid from incoming request
            string data = $"{APPId}{requestHttpMethod}{requestUri}{requestTimeStamp}{requestContentBase64String}";

            string userSecretKey = _repository.GetAll().Where(u => u.UserAppId == APPId).FirstOrDefault().UserSecretKey;

            byte[] secretKeyBytes = Convert.FromBase64String(userSecretKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            //Hash and ordinal comparison of 'incoming' & 'inner' signatures
            using (HMACSHA1 hmac = new HMACSHA1(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }

        }


        #endregion
        #endregion

        #region utils

        #region private



        private string[] GetAuthHeaderValues(string authHeader)
        {

            string[] credtiantialsArray = authHeader.Split(':');

            if (credtiantialsArray.Length == 3)
            {
                return credtiantialsArray;
            }
            else
            {
                return null;
            }

        }

        private bool IsTimeOut(string requestTimeStamp)
        {



            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAge)
            {
                return true;
            }



            return false;
        }

        private static async Task<byte[]> CreateMD5Hash(HttpContent httpContent)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = null;
                var content = await httpContent.ReadAsByteArrayAsync();
                if (content.Length != 0)
                {
                    hash = md5.ComputeHash(content);
                }
                return hash;
            }
        }

        private bool IsExisting(string appId)
        {
            if (_repository.GetAll().Where(u => u.UserAppId == appId).Count() != 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #endregion
    }
}