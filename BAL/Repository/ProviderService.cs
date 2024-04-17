using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class ProviderService : IProviders
    {
        private readonly ApplicationDbContext _context;
        private readonly IUploadProvider _uploadProvider;
        public ProviderService(ApplicationDbContext context, IUploadProvider uploadProvider)
        {
            _context = context;
            this._uploadProvider = uploadProvider;
        }
        public List<AdminProvidersVM> getPhysicianList(int stateid)
        {
            //var result = (from phy in _context.Physicians
            //             join role in _context.Roles
            //             on phy.RoleId equals role.RoleId
            //             orderby phy.CreatedDate
            //             select phy).ToList();
            var result = (from phy in _context.Physicians
                          join role in _context.Roles
                          on phy.RoleId equals role.RoleId
                          join notify in _context.PhysicianNotifications
                          on phy.PhysicianId equals notify.PhysicianId
                          where (stateid == 0 || phy.RegionId == stateid)
                          orderby phy.CreatedDate
                          select new AdminProvidersVM
                          {
                              Name = phy.FirstName,
                              Role = role.Name,
                              OnCallStatus = new BitArray(new[] { notify.IsNotificationStopped[0] }),
                              Status = phy.Status,
                              resions = _context.Regions.ToList(),
                              physicianid = phy.PhysicianId
                          }).ToList();
            return result;            
        }

        public List<AdminProvidersVM> getfilteredPhysicians(int stateid)
        {
            List<AdminProvidersVM> res = new List<AdminProvidersVM>();
            if (stateid > 0)
            {
                res = getPhysicianList(stateid).ToList();
            }
            else
            {
                res = getPhysicianList(stateid).ToList();
            }
            return res;
        }

        public Physician? getPhysicianById(int id)
        {
            return _context.Physicians.FirstOrDefault(item => item.PhysicianId == id);
        }

        public List<PhysicianRegion> getList(int id)
        {
            return _context.PhysicianRegions.Where(item => item.PhysicianId == id).ToList();
        }

        public AspNetUser getAccByEmail(string email)
        {
            return _context.AspNetUsers.FirstOrDefault(item => item.Email == email);
        }
        public void UpdatePassword(AspNetUser user,string password)
        {
            user.PasswordHash = password;
            _context.AspNetUsers.Update(user);
            _context.SaveChanges();
        }

        public void updatePhysicianInfo(Physician physician, string MobileNo, string[] Region, string SynchronizationEmail, string NPINumber, string MedicalLicense)
        {
            physician.Mobile = MobileNo;
            physician.Npinumber = NPINumber;
            physician.MedicalLicense = MedicalLicense;
            physician.SyncEmailAddress = SynchronizationEmail;
            _context.Physicians.Update(physician);
            _context.SaveChanges();


            List<PhysicianRegion> region = _context.PhysicianRegions.
                Where(item => item.PhysicianId == physician.PhysicianId).ToList();

            _context.PhysicianRegions.RemoveRange(region);
            _context.SaveChanges();

            foreach (var item in Region)
            {
                PhysicianRegion physicianRegion = new PhysicianRegion();
                physicianRegion.PhysicianId = physician.PhysicianId;
                physicianRegion.RegionId = int.Parse(item);
                _context.Add(physicianRegion);
                _context.SaveChanges();
            }
        }

        public void updateBilling(Physician physician)
        {
            _context.Physicians.Update(physician);
            _context.SaveChanges();
        }



        public void createproviderAcc(PhysicianProfileVM model, string[] Region, string password)
        {
            if(model.isFromUserAccess == null)
            {
                model.isFromUserAccess = false;
            }

            AspNetUser user = new AspNetUser();
            user.Id = Guid.NewGuid().ToString();
            user.UserName = model.FirstName + model.LastName;
            user.PasswordHash = password;
            user.Email = model.Username;
            user.PhoneNumber = model.MobileNo;
            user.CreatedDate = DateTime.Now;
            _context.AspNetUsers.Add(user);
            _context.SaveChanges();

            var aspnetuserrole = new AspNetUserRole
            {
                UserId = user.Id,
                RoleId = 3
            };
            _context.AspNetUserRoles.Add(aspnetuserrole);
            _context.SaveChanges();

            Physician physician = new Physician();
            physician.AspNetUserId = user.Id;
            physician.FirstName = model.FirstName;
            physician.LastName = model.LastName;
            physician.Email = model.Username;
            physician.Mobile = model.MobileNo;
            physician.MedicalLicense = model.MedicalLicense;
            physician.IsCredentialDoc = new BitArray(new[] { false });
            physician.IsLicenseDoc = new BitArray(new[] { false });
            physician.Address1 = model.Address1;
            physician.Address2 = model.Address2;
            physician.City = model.City;
            physician.RegionId = model.State;
            physician.Zip = model.ZipCode;
            physician.CreatedDate = DateTime.Now;
            physician.Status = 1;
            physician.BusinessName = model.BusinessName;
            physician.BusinessWebsite = model.BusinessWebsite;
            physician.RoleId = 2;
            physician.Npinumber = model.NPINumber;
            physician.Signature = model.SignatureFilename;
            _context.Physicians.Add(physician);
            _context.SaveChanges();

            if (model.File != null)
            {
                _uploadProvider.UploadPhoto(model.File, physician.PhysicianId);
                physician.Photo = model.File.FileName;
            }

            if (model.ICAFile != null)
            {
                _uploadProvider.UploadDocFile(model.ICAFile, physician.PhysicianId, "ICA");
                physician.IsAgreementDoc = new BitArray(new[] { true });
            }
            else
            {
                physician.IsAgreementDoc = new BitArray(new[] { false });
            }
            if (model.BackFile != null)
            {
                _uploadProvider.UploadDocFile(model.BackFile, physician.PhysicianId, "BackDoc");
                physician.IsBackgroundDoc = new BitArray(new[] { true });
            }
            else
            {
                physician.IsBackgroundDoc = new BitArray(new[] { false });
            }
            if (model.HippaFile != null)
            {
                _uploadProvider.UploadDocFile(model.HippaFile, physician.PhysicianId, "TrainingDoc");
                physician.IsTrainingDoc = new BitArray(new[] { true });
            }
            else
            {
                physician.IsTrainingDoc = new BitArray(new[] { false });
            }
            if (model.NonFile != null)
            {
                _uploadProvider.UploadDocFile(model.NonFile, physician.PhysicianId, "NonDisclosureDoc");
                physician.IsNonDisclosureDoc = new BitArray(new[] { true });
            }
            else
            {
                physician.IsNonDisclosureDoc = new BitArray(new[] { false });
            }
            _context.Physicians.Update(physician);
            _context.SaveChanges();

            foreach (var item in Region)
            {
                PhysicianRegion physicianRegion = new PhysicianRegion();
                physicianRegion.PhysicianId = physician.PhysicianId;
                physicianRegion.RegionId = int.Parse(item);
                _context.Add(physicianRegion);
                _context.SaveChanges();
            }

            PhysicianNotification notification = new PhysicianNotification
            {
                PhysicianId = physician.PhysicianId,
                IsNotificationStopped = new BitArray(new[] { false })
            };
            _context.PhysicianNotifications.Add(notification);
            _context.SaveChanges();

        }

        public bool changeNotification(bool ischecked, string id)
        {
            bool res;
            var phynoty = _context.PhysicianNotifications.Where(u => u.PhysicianId == int.Parse(id)).FirstOrDefault();
            if (phynoty != null)
            {
                if (ischecked == true)
                {
                    phynoty.IsNotificationStopped = new BitArray(new[] { true });
                }
                else
                {
                    phynoty.IsNotificationStopped = new BitArray(new[] { false });
                }

                _context.PhysicianNotifications.Update(phynoty);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void createAdminAcc(AdminCreateAccVM model, string password, int[] adminRegion)
        {
            AspNetUser user = new AspNetUser();
            user.Id = Guid.NewGuid().ToString();
            user.UserName = model.FirstName + model.LastName;
            user.PasswordHash = password;
            user.Email = model.Username;
            user.PhoneNumber = model.MobileNo;
            user.CreatedDate = DateTime.Now;
            _context.AspNetUsers.Add(user);
            _context.SaveChanges();

            var aspnetuserrole = new AspNetUserRole
            {
                UserId = user.Id,
                RoleId = 1
            };
            _context.AspNetUserRoles.Add(aspnetuserrole);
            _context.SaveChanges();

            Admin admin = new Admin();
            admin.Email = model.Email;
            admin.AspNetUserId = user.Id;
            admin.FirstName = model.FirstName;
            admin.LastName = model.LastName;
            admin.Mobile = model.MobileNo;
            admin.Address2 = model.Address2;
            admin.Address1 = model.Address1;
            admin.City = model.City;
            admin.RegionId = model.State;
            admin.Zip = model.ZipCode;
            admin.CreatedBy = user.Id;
            admin.Status = 1;

            _context.Admins.Add(admin);
            _context.SaveChanges();

            foreach (var region in adminRegion)
            {

                AdminRegion adminregions = new AdminRegion();
                adminregions.AdminId = admin.AdminId;
                adminregions.RegionId = region;
                _context.AdminRegions.Add(adminregions);
                _context.SaveChanges();
            }
        }
    }
}
