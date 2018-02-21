using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using testawsauth.Controllers;
using testawsauth.Models;
using System.Web.Http.Results;
using System.Net.Http;
using awsauth.Utils;

namespace awsauth
{
    [TestClass]
    public class UnitTestClient
    {

        private IUserRepository _repository;

        public UnitTestClient()
        {
            _repository = new UserRepository();
        }

        [TestMethod]
        public void TestAuthenticateUserinvalidPwd()
        {
            var controller = new AuthenticateController(_repository);

            var result = controller.Get("user1@mail.com", "user1") as OkNegotiatedContentResult<Boolean>;

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Content);
        }

        [TestMethod]
        public void TestAuthenticateUserValid()
        {
            var controller = new AuthenticateController(_repository);

            var result = controller.Get("user1@mail.com", "pwd1") as OkNegotiatedContentResult<Boolean>;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Content);
        }

        //Testing a client request
        //assuming that the server has provided an APPid & Secret to the client        
        [TestMethod]
        public void TestConfidentialWithAuth()
        {
            HttpResponseMessage response = null;

            string apiBaseAddress = "http://localhost:30490/";

            User User1 = GetOneUser(apiBaseAddress, "user1@mail.com", "pwd1");
           
            if (User1 != null)
            {
                HmacHeaderDelegatingHandler customDelegatingHandler = new HmacHeaderDelegatingHandler(User1.UserAppId, User1.UserSecretKey);

                HttpClient client = HttpClientFactory.Create(customDelegatingHandler);

                response = client.GetAsync(apiBaseAddress + "api/Confidentials").Result;

            }

           
            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.Content.ReadAsAsync<bool>().Result);

        }


        #region utils
        private User GetOneUser(string apiBaseAddress, string mail, string pwd)
        {
            User _user = null;
            HttpClient cli = new HttpClient();


            string path = $"{apiBaseAddress}api/Authenticate/GetOne?UserMail={mail}&UserPassword={pwd}";

            HttpResponseMessage response = cli.GetAsync(path).Result;

            if (response.IsSuccessStatusCode)
            {
                _user = response.Content.ReadAsAsync<User>().Result;
            }

            return _user;
        }

        #endregion

    }


    
}

