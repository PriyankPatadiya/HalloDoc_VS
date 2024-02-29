using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
                              address = reqclient.City + " , " + reqclient.State + " , " + reqclient.ZipCode.ToString(),
                              region = reqclient.State,
                              ConfirmationNumber = req.ConfirmationNumber,
                              RequestClientId = reqclient.RequestClientId,
                              
                          });
            result = result.Where(s => s.RequestClientId == reqclientId);
            return result;
        }

        public void changeStatusOnCancleCase(int requesid, string reason, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(h => h.RequestId == requesid);


            if (request != null)
            {
                request.Status = 3;
                request.CaseTag = reason;


                RequestStatusLog requeststatuslog = new RequestStatusLog();

                requeststatuslog.RequestId = requesid;
                requeststatuslog.Notes = Notes;
                requeststatuslog.CreatedDate = DateTime.Now;
                requeststatuslog.Status = 3;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                _context.Update(request);
                _context.SaveChanges();
            }
        }
    }
}
