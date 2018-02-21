using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testawsauth.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserMail { get; set; }
        public string UserPassword { get; set; }
        public string UserAppId { get; set; }
        public string UserSecretKey { get; set; }
        public bool UserIsAuthenticated { get; set; }
    }
}