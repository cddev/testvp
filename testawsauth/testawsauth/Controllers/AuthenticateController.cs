using System.Linq;
using System.Web.Http;
using testawsauth.Models;

namespace testawsauth.Controllers
{

    [RoutePrefix("api/Authenticate")]
    public class AuthenticateController : ApiController
    {
        private IUserRepository db;

        public AuthenticateController(IUserRepository context)
        {
            db = context;

        }

        [Route("")]
        public IHttpActionResult Get([FromUri]string UserMail, string UserPassword) 
        {


            var user = db.GetAll().Where(u => u.UserMail == UserMail && u.UserPassword == UserPassword).FirstOrDefault();

            if (user != null)
            {
                user.UserIsAuthenticated = true;
                db.Update(user);
                return Ok(user.UserIsAuthenticated);
            }
            else
            {
                return Ok(false);
            }


        }


        #region utils
        [Route("GetOne")]
        [HttpGet]
        public IHttpActionResult GetOne([FromUri]string UserMail, string UserPassword)
        {


            var user = db.GetAll().Where(u => u.UserMail == UserMail && u.UserPassword == UserPassword).FirstOrDefault();

            if (user != null)
            {
                user.UserIsAuthenticated = true;
                db.Update(user);
                return Ok(user);
            }
            else
            {
                return Ok();
            }


        }
        #endregion




    }
}
