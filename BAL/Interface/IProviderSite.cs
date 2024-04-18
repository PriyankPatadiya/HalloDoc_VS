using DAL.DataModels;
using DAL.ViewModels;


namespace BAL.Interface
{
    public interface IProviderSite
    {
        public int CountRequests(string StatusButton, int? physicianId);
        public PhysicianProfileVM getProviderProfileData(Physician? physician);
        public List<PartnersVM> getVendors(string search, string regionId);
        public void addBusiness(AddBusinessVM vendor);
        public AddBusinessVM getBusinessDetails(int Vendorid);
        public void updateBusinessDetails(AddBusinessVM vendor, HealthProfessional Vendor);
        public List<User> getFileteredPatient(string firstName, string lastName, string email, string phoneNumber);
        public List<explorePatientVM> getExploreRequests(int userId);
        public IEnumerable<SearchRecordsVM> getSearchRecordsWithFilter(int[] status, string patientName,
        string providerName, string phoneNum, string email, string requestType, DateTime fromDate, DateTime toDate);
        public IQueryable<BlockedHistoryVM> getBlockedRequests();
        public void deleteBusiness(HealthProfessional vendor);
    }
}
