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
    }
}
