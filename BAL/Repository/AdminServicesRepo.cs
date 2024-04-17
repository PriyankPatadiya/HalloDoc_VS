using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;
using DAL.DataModels;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Ocsp;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace BAL.Repository
{
    public class AdminServicesRepo : IAdminDashboard
    { 
        private readonly IUploadProvider _uploadProvider;
        private readonly ApplicationDbContext _context;

        public AdminServicesRepo(ApplicationDbContext context , IUploadProvider upload)
        {
             _context = context;
             _uploadProvider = upload;
        }

        public Request reqbyreqid(int id)
        {
            var request = _context.Requests.Where(i => i.RequestId == id).FirstOrDefault();
            return request;
        }
        public string username(string email)
        {
            string username = _context.Admins.First(u => u.Email == email).FirstName;
            return username;
        }

        public List<Region> regions()
        {
            var regions = _context.Regions.ToList();
            return regions;
        }

        public List<RequestWiseFile> filesbyrequestid(int requestid)
        {
            var requestfiles = _context.RequestWiseFiles.Where(x => x.RequestId == requestid).ToList();
            return requestfiles;
        }

        public RequestWiseFile filebyName(string name)
        {
            var reqwisefile = _context.RequestWiseFiles.Where(u => u.FileName == name).FirstOrDefault();
            return reqwisefile;
        }

        public RequestWiseFile filebyReqidandName(int reqid, string filename)
        {
            var requestwisefile = _context.RequestWiseFiles.Where(u => u.RequestId == reqid && u.FileName == filename).FirstOrDefault();
            return requestwisefile;
        }

        public List<HealthProfessional> getprofessionalsbyvendorid(string businessId)
        {
            var data = _context.HealthProfessionals.Where(u => u.VendorId == int.Parse(businessId));
            return data.ToList();
        }

        public void deleteSingleFile(RequestWiseFile file)
        {
            file.IsDeleted[0] = true;
            _context.RequestWiseFiles.Update(file);
            _context.SaveChanges();
        }

        public bool deleteAllFiles(List<string> selectedFiles)
        {
            foreach (var file in selectedFiles)
            {
                if(file == null)
                {
                    return false;
                }
            }
            foreach (var file in selectedFiles)
            {
                var reqwisefile = filebyName(file);
                reqwisefile.IsDeleted[0] = true;
                _context.RequestWiseFiles.Update(reqwisefile);
            }
            _context.SaveChanges();
            return true;
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
                         where !(req.IsDeleted == new BitArray(new bool[] { false }))
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
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                          on req.RequestId equals reqclient.RequestId
                          where ! (req.IsDeleted == new BitArray(new bool[] { false }))
                          orderby req.CreatedDate
                          select new AdminDashboardTableVM()
                          {
                              PatientName = reqclient.FirstName + " " + reqclient.LastName,
                              BirthDate = new DateOnly((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate).ToString("MMMM dd, yyyy"),
                              RequestorName = req.FirstName + " " + req.LastName,
                              RequestDate = req.CreatedDate.ToString("MMMM dd,yyyy hh:mm"),
                              phone = reqclient.PhoneNumber,
                              address = reqclient.Street + " ," + reqclient.City + " ," + reqclient.State + ", " + reqclient.ZipCode,
                              requestId = req.RequestTypeId,
                              YourPhone = req.PhoneNumber,
                              requestStatus = req.Status,
                              Email = reqclient.Email,
                              reqclientId = reqclient.RequestClientId,
                              regionId = reqclient.RegionId,
                              Region = _context.Regions.FirstOrDefault(u => u.RegionId == reqclient.RegionId).Name,
                              reqid = req.RequestId,
                              regionTable = _context.Regions.ToList(),
                              DateOfService = req.ModifiedDate,
                              physicianname = _context.Physicians.FirstOrDefault(u => u.PhysicianId == req.PhysicianId).FirstName,
                              IsEncounterFinalize = _context.EncounterForms.FirstOrDefault(r => r.RequestId == req.RequestId).IsFinalize,
                              physicianId = req.PhysicianId,
                              Notes = _context.RequestStatusLogs.Where(u => u.RequestId == req.RequestId && myarray.Any(s=>u.Status == s)).FirstOrDefault().Notes

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

        public AdminProfileVM getProfileData(string email)
        {
            AdminProfileVM profile = (from admin in _context.Admins
                                     join AspNetUser in _context.AspNetUsers
                                     on admin.AspNetUserId equals AspNetUser.Id
                                     join adminregion in _context.AdminRegions
                                     on admin.AdminId equals adminregion.AdminId into adminRegion
                                     from totalAdminRegion in adminRegion.DefaultIfEmpty()
                                     where admin.Email == email
                                     select new AdminProfileVM()
                                     {
                                         UserName = admin.Email,
                                         Password = AspNetUser.PasswordHash,
                                         FirstName = admin.FirstName,
                                         LastName = admin.LastName,
                                         Email = admin.Email,
                                         Confirmemail = admin.Email,
                                         phone = admin.Mobile,
                                         Address1 = admin.City +", "+ _context.Regions.First(u => u.RegionId == admin.RegionId).Name,
                                         Address2 = "---",
                                         city = admin.City,
                                         state = _context.Regions.First(u => u.RegionId == admin.RegionId).RegionId,
                                         zipcode = admin.Zip,
                                         billingphone = admin.AltPhone,
                                         statess = (from adminregion in _context.AdminRegions where adminregion.AdminId == admin.AdminId select new CheckboxList_model
                                         {
                                             Value = adminregion.RegionId,
                                             Selected = true
                                         }).ToList(),

                                     }).First();
            profile.Region = _context.Regions.ToList();
            return profile;
        }

        public void changeAccountInfo(AdminProfileVM model, string email, List<string> regions)
        {
            var admin = _context.Admins.First(u => u.Email == email);
            var aspnetuser = _context.AspNetUsers.First(u => u.Email == email);
            var List = _context.AdminRegions.Where(s => s.AdminId == admin.AdminId).ToList();

            _context.AdminRegions.RemoveRange(List);

            //add on the adminid 
            foreach (var item in regions)
            {
                AdminRegion adminRegion = new AdminRegion();
                adminRegion.AdminId = admin.AdminId;
                adminRegion.RegionId = int.Parse(item);
                _context.Add(adminRegion);
                _context.SaveChanges();
            }

            
            admin.Email = model.Email;
            admin.FirstName = model.FirstName;
            admin.LastName = model.LastName;
            admin.Mobile = model.phone;
            _context.Admins.Update(admin);
            _context.SaveChanges();

            
            aspnetuser.Email = model.Email;
            _context.AspNetUsers.Update(aspnetuser);
            _context.SaveChanges();
        }

        public void changeBillingInfo(AdminProfileVM model, string email)
        {
            var admin = _context.Admins.First(u => u.Email == email);
            admin.Address1 = model.Address1;
            admin.Address2 = model.Address2;
            admin.City = model.city;
            admin.RegionId = model.SelectedStateId;
            admin.Zip = model.zipcode;
            admin.AltPhone = model.billingphone;
            _context.Admins.Update(admin);
            _context.SaveChanges();             
            
            var adminregion = _context.AdminRegions.First(u => u.AdminId == admin.AdminId);
            adminregion.RegionId = model.SelectedStateId;
            _context.AdminRegions.Update(adminregion);
            _context.SaveChanges();
        }

        public void changePassword(string email, string password)
        {
            var admin = _context.AspNetUsers.First(u => u.Email == email);
            if(admin != null)
            {
                admin.PasswordHash = password;
                _context.AspNetUsers.Update(admin);
                _context.SaveChanges();
            }
        }

        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile)
        {
            var physician = _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);

            if (physician != null)
            {
                physician.BusinessName = businessName;
                physician.BusinessWebsite = businessWebsite;

                if (signatureFile != null && signatureFile.FileName != null)
                {
                    string signatureFileName = _uploadProvider.UploadSignature(signatureFile, id);
                    physician.Signature = signatureFileName;
                }

                if (photoFile != null && photoFile.FileName != null)
                {
                    string photoFileName = _uploadProvider.UploadPhoto(photoFile, id);
                    physician.Photo = photoFileName;
                }

                _context.Physicians.Update(physician);
                _context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Physician not found");
            }
        }
    }
}
