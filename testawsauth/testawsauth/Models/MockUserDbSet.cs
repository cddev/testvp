using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;


namespace testawsauth.Models
{
    class MockUserDbSet : MockDbSet<User>
    {

        public MockUserDbSet()
        {
           
        }
        public override User Add(User item)
        {
            return base.Add(item);
        }

        public override User Find(params object[] keyValues)
        {
            return base.Find(keyValues);
        }

       
    }
}