using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IProviders
    {
        public List<AdminProvidersVM> getPhysicianList(int stateid);
        public List<AdminProvidersVM> getfilteredPhysicians(int stateid);
        public Physician? getPhysicianById(int id);
        public List<PhysicianRegion> getList(int id);
        public AspNetUser getAccByEmail(string email);
        public void UpdatePassword(AspNetUser user, string password);
        public void updatePhysicianInfo(Physician physician, string MobileNo, string[] Region, string SynchronizationEmail, string NPINumber, string MedicalLicense);
        public void updateBilling(Physician physician);
    }
}
