using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testawsauth.Models
{
    public interface IUserRepository
    {
        IEnumerable<User> GetAll();
        User GetByID(int id);
        void Add(User user);
        void Update(User user);
    }
}
