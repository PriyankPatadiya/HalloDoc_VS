using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class ViewCaseAdminRepo : IViewCaseAdmin
    {
        private readonly ApplicationDbContext _context;

        public ViewCaseAdminRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<ViewCaseVM> getViewCaseData(int reqclientId)
        {
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                           on req.RequestId equals reqclient.RequestId
                            select new ViewCaseVM()
                          {
                              FirstName = reqclient.FirstName.ToLower(),
                              LastName = reqclient.LastName,
                              DateOfBirth = (new DateTime((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate)),
                              Phone = reqclient.PhoneNumber,
                              Email = reqclient.Email,
                              Notes = reqclient.Notes,
                              address = reqclient.City + reqclient.State + reqclient.ZipCode.ToString(),
                              region = reqclient.State,
                              ConfirmationNumber = req.ConfirmationNumber,
                              RequestClientId = reqclient.RequestClientId,

                          });
            result = result.Where(s => s.RequestClientId == reqclientId);
            return result;
        }
    }
}
