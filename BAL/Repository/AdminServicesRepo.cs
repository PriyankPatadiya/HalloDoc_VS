using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class AdminServicesRepo : IAdminDashboard
    { 

        private readonly ApplicationDbContext _context;

        public AdminServicesRepo(ApplicationDbContext context)
        {
             _context = context;
        }


        public int CountRequests(string StatusButton)
        {
            List<int> myarray = new List<int>();

            if (StatusButton == "1")
            {
                myarray.Add(1);
            }
            if (StatusButton == "2")
            {
                myarray.Add(2);
            }
            if (StatusButton == "3")
            {
                myarray.Add(4);
                myarray.Add(5);
            }
            if (StatusButton == "4")
            {
                myarray.Add(6);
            }
            if (StatusButton == "5")
            {
                myarray.Add(3);
                myarray.Add(7);
                myarray.Add(8);
            }
            if (StatusButton == "6")
            {
                myarray.Add(9);
            }
            var count = (from req in _context.Requests
                         join reqclient in _context.RequestClients
                         on req.RequestId equals reqclient.RequestId
                         select req.Status ).Where(item => myarray.Any(s => item == s)).Count();
            return count;
        }


        public IQueryable<AdminDashboardTableVM> GetRequestsQuery(string status)
        {
            List<int> myarray = new List<int>();

            if (status == "1")
            {
                myarray.Add(1);
            }
            if (status== "2") {
                myarray.Add(2);
            }
            if(status == "3")
            {
                myarray.Add(4);
                myarray.Add(5);
            }
            if(status == "4")
            {
                myarray.Add(6);
            }
            if(status == "5")
            {
                myarray.Add(3);
                myarray.Add(7);
                myarray.Add(8);
            }
            if(status == "6")
            {
                myarray.Add(9);
            }
           var result =  ( from req in _context.Requests
                          join reqclient in _context.RequestClients
                          on req.RequestId equals reqclient.RequestId
                          select new AdminDashboardTableVM()
                          {

                              PatientName = reqclient.FirstName + " " + reqclient.LastName,
                              BirthDate = new DateOnly((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate).ToString("MMMM dd, yyyy"),
                              RequestorName = req.FirstName + " " + req.LastName,
                              RequestDate = req.CreatedDate.ToString("MMMM dd , yyyy hh mm"),
                              phone = reqclient.PhoneNumber,
                              address = reqclient.Street + " ," + reqclient.City + " ," + reqclient.State + ", " + reqclient.ZipCode,
                              requestId = req.RequestTypeId,
                              YourPhone = req.PhoneNumber,
                              requestStatus = req.Status,
                              Email = reqclient.Email,
                              reqclientId = reqclient.RequestClientId,
                              regionId = reqclient.RegionId,
                              reqid = req.RequestId

                          }).Where(item=>myarray.Any(s=>item.requestStatus==s));
            
            
            return result;
        }

        public AdminDashboardTableVM GetRequestModel()
        {
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                          on req.RequestId equals reqclient.RequestId
                          select new AdminDashboardTableVM()
                          {

                              PatientName = reqclient.FirstName + " " + reqclient.LastName,
                              BirthDate = new DateOnly((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate).ToString("MMMM dd, yyyy"),
                              RequestorName = req.FirstName + " " + req.LastName,
                              RequestDate = req.CreatedDate.ToString("MMMM dd , yyyy hh mm"),
                              phone = reqclient.PhoneNumber,
                              address = reqclient.Street + " ," + reqclient.City + " ," + reqclient.State + ", " + reqclient.ZipCode,
                              requestId = req.RequestTypeId,
                              YourPhone = req.PhoneNumber,
                              requestStatus = req.Status,
                          });
            return (AdminDashboardTableVM)result;
        }

        public int getRequestIdbyRequestClientId(int requestClientId)
        {
            int requestID = _context.RequestClients.Where(s => s.RequestClientId == requestClientId).FirstOrDefault().RequestId; 
            return requestID;
        }

        public int getStatusByRequetId(int requetId)
        {
            int status = _context.Requests.Where(s => s.RequestId == requetId).FirstOrDefault().Status; 
            return status;
        }
    }
}
