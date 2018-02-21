using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testawsauth.Models
{
    public class UserRepository : IDisposable, IUserRepository
    {
        private UserContext db = new UserContext();

        public void Add(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        public IEnumerable<User> GetAll()
        {
            return db.Users;
        }

        public User GetByID(int id)
        {
            return db.Users.FirstOrDefault(u => u.UserId == id);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Update(User user)
        {
            db.MarkAsModified(user);
            db.SaveChanges();
        }
    }
}