using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{

    // LoginRepository Contains LoginVarify method defination. 

    public class LoginRepo : ILogin
    {
        private readonly ApplicationDbContext _context;

        public LoginRepo(ApplicationDbContext context)
        {
            _context = context;
        } 

        public bool LoginVarify(LoginVM model)
        {
            var user = _context.AspNetUsers.Any(u => u.Email == model.Email);  
            if(user != null && _context.AspNetUsers.FirstOrDefault(u => u.PasswordHash == model.PasswordHash) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
