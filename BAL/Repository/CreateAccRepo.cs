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
    public class CreateAccRepo : ICreateAcc
    {
        private readonly ApplicationDbContext _context;
        public CreateAccRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddUser(AspNetUser model)
        {
            _context.AspNetUsers.Add(model);
            _context.SaveChanges();
        }
    }
}
