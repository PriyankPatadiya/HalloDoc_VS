﻿using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections;

namespace BAL.Repository
{
    public class PatientRequestRepo : IPatientRequest
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<PatientReqVM> _passwordHasher;
        public PatientRequestRepo(ApplicationDbContext context, IPasswordHasher<PatientReqVM> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public RequestClient reqclientByRequestId(int requsestId)
        {
            return _context.RequestClients.Where(u => u.RequestId == requsestId).FirstOrDefault();
        }
        public bool isReqClientExist(int requsestId)
        {
            return _context.RequestClients.Any(u => u.RequestId == requsestId);
        }
        public List<RequestWiseFile> requestWiseFilesById(int requsestId)
        {
            return _context.RequestWiseFiles.Where(x => x.RequestId == requsestId).ToList();
        }
        public DAL.DataModels.Request requestByRequestId(int requsestId)
        {
            return _context.Requests.Where(u => u.RequestId == requsestId).FirstOrDefault();
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
                aspnetuser.PasswordHash = _passwordHasher.HashPassword(null , pInfo.PasswordHash);
                aspnetuser.PhoneNumber = pInfo.PhoneNumber;
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
                user.RegionId = pInfo.SelectedStateId;
                _context.Users.Add(user);
                _context.SaveChanges();

                var aspnetuserrole = new AspNetUserRole
                {
                    UserId = aspnetuser.Id,
                    RoleId = 2
                };
                _context.AspNetUserRoles.Add(aspnetuserrole);
                _context.SaveChanges();

                var request = new DAL.DataModels.Request();
                request.RequestTypeId = 1;
                request.UserId = user.UserId;
                request.FirstName = pInfo.FirstName;
                request.LastName = pInfo.LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = pInfo.PhoneNumber;
                request.Email = pInfo.Email;
                request.ConfirmationNumber = pInfo.confirmationnumber;
                request.IsDeleted = new BitArray(new bool[] { false });
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
                requestClient.IntDate = user.IntDate;
                requestClient.IntYear = user.IntYear;
                requestClient.StrMonth = user.StrMonth;
                requestClient.RegionId = user.RegionId;

                _context.RequestClients.Add(requestClient);
                _context.SaveChanges();

                var reqnotes = new RequestNote();
                reqnotes.RequestId = request.RequestId;
                reqnotes.AdminNotes = "-";
                reqnotes.CreatedDate = DateTime.Now;
                reqnotes.CreatedBy = request.FirstName + request.LastName;
                reqnotes.PhysicianNotes = "-";
                _context.RequestNotes.Add(reqnotes);
                _context.SaveChanges();
            }
            else
            {
                var request = new DAL.DataModels.Request();

                request.UserId = status.UserId;
                request.FirstName = pInfo.FirstName;
                request.LastName = pInfo.LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = pInfo.PhoneNumber;
                request.Email = pInfo.Email;
                request.ConfirmationNumber = pInfo.confirmationnumber;
                request.IsDeleted = new BitArray(new bool[] { false });
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
                requestClient.IntDate = pInfo.BirthDate.Value.Day;
                requestClient.IntYear = pInfo.BirthDate.Value.Year;
                requestClient.StrMonth = pInfo.BirthDate.Value.Month.ToString();
                requestClient.RegionId = pInfo.SelectedStateId;


                _context.RequestClients.Add(requestClient);
                _context.SaveChanges();

                var reqnotes = new RequestNote();
                reqnotes.RequestId = request.RequestId;
                reqnotes.AdminNotes = "-";
                reqnotes.CreatedDate = DateTime.Now;
                reqnotes.CreatedBy = request.FirstName + request.LastName;
                reqnotes.PhysicianNotes = "-";
                _context.RequestNotes.Add(reqnotes);
                _context.SaveChanges();
            }

        }
       
        public void AddRequestForElse(PatientReqVM pInfo)
        {
            var request = new DAL.DataModels.Request();

            request.RequestTypeId = 2;
            request.FirstName = pInfo.FirstName;
            request.LastName = pInfo.LastName;
            request.CreatedDate = DateTime.Now;
            request.PhoneNumber = pInfo.PhoneNumber;
            request.Email = pInfo.Email;
            request.ConfirmationNumber = pInfo.confirmationnumber;
            request.IsDeleted = new BitArray(new bool[] { false });
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
            requestClient.IntDate = pInfo.BirthDate.Value.Day;
            requestClient.IntYear = pInfo.BirthDate.Value.Year;
            requestClient.StrMonth = pInfo.BirthDate.Value.Month.ToString();
            requestClient.RegionId = pInfo.SelectedStateId;


            _context.RequestClients.Add(requestClient);
            _context.SaveChanges();

            var reqnotes = new RequestNote();
            reqnotes.RequestId = request.RequestId;
            reqnotes.AdminNotes = "-";
            reqnotes.CreatedDate = DateTime.Now;
            reqnotes.CreatedBy = request.FirstName + request.LastName;
            reqnotes.PhysicianNotes = "-";
            _context.RequestNotes.Add(reqnotes);
            _context.SaveChanges();
        }
        public RequestClient GetUserByEmail(string email)
        {
            return _context.RequestClients.OrderBy(e => e.RequestId).LastOrDefault(u => u.Email == email);
        }

        public void Addrequestwisefile(string filename, int requestid)
        {
            BitArray isdelete = new BitArray(1);
            isdelete[0] = false;
            var requestwisefile = new RequestWiseFile
            {
                RequestId = requestid,
                FileName = filename,
                CreatedDate = DateTime.Now,
                IsDeleted = isdelete
            };

            _context.Add(requestwisefile);
            _context.SaveChanges();
        }

        public void AddAdminCreateRequest(PatientReqVM pInfo, string email, bool isPhysician, int physicianID)
        {

            
            if(pInfo != null)
            {
                pInfo.CreatedDate = DateTime.Now;
                var request = new DAL.DataModels.Request();
                request.Status = (short)(isPhysician ? 2 : 1);
                if (isPhysician)
                {
                    request.PhysicianId = physicianID;
                }
                request.FirstName = isPhysician ? _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianID).FirstName : _context.Admins.Where(x => x.Email == email).FirstOrDefault().FirstName;
                request.LastName = isPhysician ? _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianID).LastName : _context.Admins.Where(x => x.Email == email).FirstOrDefault().LastName;
                request.CreatedDate = DateTime.Now;
                request.PhoneNumber = isPhysician ? _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianID).Mobile : _context.Admins.Where(x => x.Email == email).FirstOrDefault().Mobile;
                request.Email = isPhysician ? _context.Physicians.FirstOrDefault(x => x.PhysicianId == physicianID).Email : email;
                request.ConfirmationNumber = GenerateConfirmationNumber(pInfo);
                request.IsDeleted = new BitArray(new bool[] { false });

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
                requestClient.IntDate = pInfo.BirthDate.Value.Day;
                requestClient.IntYear = pInfo.BirthDate.Value.Year;
                requestClient.StrMonth = pInfo.BirthDate.Value.Month.ToString();
                requestClient.RegionId = pInfo.SelectedStateId;

                _context.RequestClients.Add(requestClient);
                _context.SaveChanges();

                var reqnotes = new RequestNote();
                reqnotes.RequestId = request.RequestId;
                if (isPhysician)
                {
                    reqnotes.PhysicianNotes = pInfo.AdminNote;
                }
                else
                {
                    reqnotes.AdminNotes = pInfo.AdminNote;

                }
                reqnotes.CreatedDate = DateTime.Now;
                reqnotes.CreatedBy = request.FirstName + request.LastName;
                _context.RequestNotes.Add(reqnotes);
                _context.SaveChanges();
            }
        }

        public async Task<string> GetStateAccordingToRegionId(int regionId)
        {
            string state =  _context.Regions.Where(s => s.RegionId == regionId).FirstOrDefault().Name;
            return state;
        }

        public string GenerateConfirmationNumber(PatientReqVM model)
        {
            string abr = _context.Regions.Where(s => s.Name == model.State).FirstOrDefault().Abbreviation;
            int numofrequests = _context.Requests.Where(s => s.CreatedDate.Date == model.CreatedDate).Count()+1;
            string confirmationnum = abr + model.CreatedDate.Value.ToString("MMdd") + model.LastName.Substring(0, 2) + model.FirstName.Substring(0, 2) + numofrequests.ToString("D4");
            return confirmationnum;
        }
    }
}
