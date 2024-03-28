using BAL.Interface;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class ProviderService : IProviders
    {
        private readonly ApplicationDbContext _context;
        public ProviderService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
