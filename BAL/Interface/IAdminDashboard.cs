using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BAL.Interface
{
    public interface IAdminDashboard
    {
        public string username(string email);
        public Request reqbyreqid(int id);
        public List<RequestWiseFile> filesbyrequestid(int requestid);
        public RequestWiseFile filebyReqidandName(int reqid, string filename);
        public RequestWiseFile filebyName(string name);
        public List<HealthProfessional> getprofessionalsbyvendorid(string businessId);
        public void deleteSingleFile(RequestWiseFile file);
        public bool deleteAllFiles(List<string> selectedFiles);
        public List<Region> regions();
        public IQueryable<AdminDashboardTableVM> GetRequestsQuery(string status);
        public AdminDashboardTableVM GetRequestModel();
        public int CountRequests(string StatusButton);
        public int getRequestIdbyRequestClientId(int requestClientId);
        public int getStatusByRequetId(int requetId);
        public AdminProfileVM getProfileData(string email);
        public void changeAccountInfo(AdminProfileVM model, string email, List<string> regions);
        public void changeBillingInfo(AdminProfileVM model, string email);
        public void changePassword(string password, string email);

        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile);
    }
}
