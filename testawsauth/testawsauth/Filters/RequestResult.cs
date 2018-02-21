using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace testawsauth.Filters
{
    public class RequestResult : IHttpActionResult
    {
        #region props

        private string AuthSchema { get; set; }
        private IHttpActionResult Next { get; set; }

        #endregion


        #region wrk
        public RequestResult(IHttpActionResult next)
        {
            this.AuthSchema = "foo";
            this.Next = next;
        }
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await Next.ExecuteAsync(cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(AuthSchema));
            }

            return response;
        }
        #endregion
    }
}