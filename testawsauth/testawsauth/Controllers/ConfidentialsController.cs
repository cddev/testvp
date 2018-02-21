using System.Linq;
using System.Net.Http;
using System.Web.Http;
using testawsauth.Models;
using testawsauth.Filters;
using System.Security.Claims;


namespace testawsauth.Controllers
{

    [AWSAuth]
    [RoutePrefix("api/Confidentials")]
    public class ConfidentialsController : ApiController
    {
        private IUserRepository _repository;

        public ConfidentialsController(IUserRepository repository)
        {
            _repository = repository;
            AWSAuthAttribute._repository = repository;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            string appId = ClaimsPrincipal.Current.Identity.Name;

            return Ok(_repository.GetAll().Where(u => u.UserAppId == appId).FirstOrDefault().UserIsAuthenticated);
        }

    }
}
