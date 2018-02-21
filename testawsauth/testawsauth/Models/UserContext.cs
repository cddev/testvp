using System;
using System.Data.Entity;
using System.Security.Cryptography;

namespace testawsauth.Models
{
    public class UserContext : IUserContext
    {
        public DbSet<User> Users { get; set; }

        public UserContext()
        {
            this.Users = new MockUserDbSet();

            for (int i = 0; i < 10; i++)
            {
                this.Users.Add(new Models.User() { UserId = i, UserMail = $"user{i.ToString()}@mail.com", UserPassword = $"pwd{i.ToString()}", UserAppId = Guid.NewGuid().ToString("N"), UserSecretKey =  GenerateSecretKey(), UserIsAuthenticated = false });
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
            GC.SuppressFinalize(this);
        }

        public void MarkAsModified(User item)
        {
            //throw new NotImplementedException();
            
           
           
        }

        public int SaveChanges()
        {
            return 0;
        }

        private string GenerateSecretKey()
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32];
                cryptoProvider.GetBytes(secretKeyByteArray);
                return Convert.ToBase64String(secretKeyByteArray);
            }
        }
    }
}