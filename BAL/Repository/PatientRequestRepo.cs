using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class PatientRequestRepo : IPatientRequest
    {
        private readonly ApplicationDbContext _context;
        public PatientRequestRepo(ApplicationDbContext context)
        {
            _context = context;
        }   
       
        // PatientForm
        public void  AddPatientForm(PatientReqVM pInfo)
        {
            var status = _context.Users.FirstOrDefault(u => u.Email == pInfo.Email);
            if (status == null)
            {
                var aspnetuser = new AspNetUser();


                aspnetuser.Id = Guid.NewGuid().ToString();
                aspnetuser.Email = pInfo.Email;
                aspnetuser.CreatedDate = DateTime.Now;
                aspnetuser.UserName = pInfo.FirstName + pInfo.LastName;
                aspnetuser.PasswordHash = pInfo.PasswordHash;


                _context.AspNetUsers.Add(aspnetuser);
                _context.SaveChanges();

                var user = new User();
                
                user.AspNetUserId = aspnetuser.Id;
                user.FirstName = pInfo.FirstName;
                user.LastName = pInfo.LastName;
                user.Mobile = pInfo.PhoneNumber;
                user.Street = pInfo.Street;
                user.City = pInfo.City;
                user.State = pInfo.State;
                user.ZipCode = pInfo.ZipCode;
                user.Email = pInfo.Email;
                user.CreatedBy = pInfo.FirstName;
                user.IntYear = pInfo.BirthDate.Value.Year;
                user.IntDate = pInfo.BirthDate.Value.Day;
                user.StrMonth = pInfo.BirthDate.Value.Month.ToString();

                _context.Users.Add(user);
                _context.SaveChanges();


                var request = new Request();

                request.UserId = user.UserId;
                request.FirstName = pInfo.FirstName;
                request.LastName = pInfo.LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = pInfo.PhoneNumber;
                request.Email = pInfo.Email;

                _context.Requests.Add(request);
                _context.SaveChanges();


                var requestClient = new RequestClient();

                requestClient.RequestId = request.RequestId;
                requestClient.FirstName = pInfo.FirstName;
                requestClient.LastName = pInfo.LastName;
                requestClient.Email = pInfo.Email;
                requestClient.PhoneNumber = pInfo.PhoneNumber;
                requestClient.Street = pInfo.Street;
                requestClient.City = pInfo.City;
                requestClient.State = pInfo.State;
                requestClient.ZipCode = pInfo.ZipCode;




                _context.RequestClients.Add(requestClient);
                _context.SaveChanges();
            }
            else
            {
                var request = new Request();

                request.UserId = status.UserId;
                request.FirstName = pInfo.FirstName;
                request.LastName = pInfo.LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = pInfo.PhoneNumber;
                request.Email = pInfo.Email;

                _context.Requests.Add(request);
                _context.SaveChanges();


                var requestClient = new RequestClient();

                requestClient.RequestId = request.RequestId;
                requestClient.FirstName = pInfo.FirstName;
                requestClient.LastName = pInfo.LastName;
                requestClient.Email = pInfo.Email;
                requestClient.PhoneNumber = pInfo.PhoneNumber;
                requestClient.Street = pInfo.Street;
                requestClient.City = pInfo.City;
                requestClient.State = pInfo.State;
                requestClient.ZipCode = pInfo.ZipCode;




                _context.RequestClients.Add(requestClient);
                _context.SaveChanges();
            }

        }
       
        public Request GetUserByEmail(string email)
        {
            return _context.Requests.OrderBy(e => e.RequestId).LastOrDefault(u => u.Email == email);
        }

        public void Addrequestwisefile(string filename, int requestid)
        {
            var requestwisefile = new RequestWiseFile
            {
                RequestId = requestid,
                FileName = filename,
                CreatedDate = DateTime.Now,
            };

            _context.Add(requestwisefile);
            _context.SaveChanges();
        }



    }
}
