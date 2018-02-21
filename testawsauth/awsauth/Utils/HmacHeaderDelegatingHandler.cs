using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace awsauth.Utils
{
    public class HmacHeaderDelegatingHandler : DelegatingHandler
    {
        #region vars
        private string APPId;
        private string APIKey;
        #endregion

        #region ctor
        public HmacHeaderDelegatingHandler(string appId, string apikey)
        {
            APPId = appId;
            APIKey = apikey;
        }
        #endregion

        #region wrk
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            HttpResponseMessage response = null;

            string requestContentBase64String = string.Empty;
            string requestSignatureBase64String = string.Empty;

            string requestUri = System.Web.HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            string requestHttpMethod = request.Method.Method;



            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();


           // string unicityGuid = Guid.NewGuid().ToString("N");

            //Request Content is hashed to MD5
            if (request.Content != null)
            {
                byte[] content = await request.Content.ReadAsByteArrayAsync();
                MD5 md5 = MD5.Create();
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }


            string StringToSign = $"{APPId}{requestHttpMethod}{requestUri}{requestTimeStamp}{requestContentBase64String}";

            byte[] secretKeyByteArray = Convert.FromBase64String(APIKey);


           
            byte[] signature = Encoding.UTF8.GetBytes(StringToSign);
            //Creating Signature by hashing the str to sign to hmac-sha1 
            using (HMACSHA1 hmac = new HMACSHA1(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                //Building Auth Header
                request.Headers.Authorization = new AuthenticationHeaderValue("foo", $"{APPId}:{requestSignatureBase64String}:{requestTimeStamp}");
            }
            //Executing Request 
            response = await base.SendAsync(request, cancellationToken);

            return response;
        }
        #endregion

    }

}
