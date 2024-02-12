using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
     public class ConciergeRequestRepo : IRequests
        {

        private readonly ApplicationDbContext _context;
        public ConciergeRequestRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddOtherForm(OthersReqVM model)
        {
            

        }
    }
}
