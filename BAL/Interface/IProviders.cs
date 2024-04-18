using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IProviders
    {
        public List<AdminProvidersVM> getPhysicianList(int stateid);
        public List<AdminProvidersVM> getfilteredPhysicians(int stateid);
        public List<HealthProfessionalType> getProfessionals();

        public HealthProfessional getProfessionByVendorId(int vendorid);
        public Physician? getPhysicianById(int id);
        public List<PhysicianRegion> getList(int id);
        public AspNetUser getAccByEmail(string email);
        public void UpdatePassword(AspNetUser user, string password);
        public void updatePhysicianInfo(Physician physician, string MobileNo, string[] Region, string SynchronizationEmail, string NPINumber, string MedicalLicense);
        public void updateBilling(Physician physician);
        public void createproviderAcc(PhysicianProfileVM model, string[] Region, string password);
        public bool changeNotification(bool ischecked, string id);
        public void createAdminAcc(AdminCreateAccVM model, string password, int[] adminRegion);
    }
}
