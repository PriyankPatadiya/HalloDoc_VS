using DAL.ViewModels;


namespace BAL.Interface
{
    public interface IAdminDashboard
    {
        public IQueryable<AdminDashboardTableVM> GetRequestsQuery(string status);
        public AdminDashboardTableVM GetRequestModel();
        public int CountRequests(string StatusButton);
        public int getRequestIdbyRequestClientId(int requestClientId);
        public int getStatusByRequetId(int requetId);
        public AdminProfileVM getProfileData(string email);
        public void changeAccountInfo(AdminProfileVM model, string email, List<string> regions);
        public void changeBillingInfo(AdminProfileVM model, string email);
        public void changePassword(string password, string email);
        
    }
}
