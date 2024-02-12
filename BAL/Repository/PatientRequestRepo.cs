using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
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
            var aspnetuser = new AspNetUser();

            aspnetuser.Id = Guid.NewGuid().ToString();
            aspnetuser.Email = pInfo.Email;
            aspnetuser.CreatedDate = DateTime.Now;
            aspnetuser.UserName = pInfo.Email;
            aspnetuser.PasswordHash = pInfo.PasswordHash;


            _context.AspNetUsers.Add(aspnetuser);
            

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

            _context.Users.Add(user);
            

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


        

        

    }
}
