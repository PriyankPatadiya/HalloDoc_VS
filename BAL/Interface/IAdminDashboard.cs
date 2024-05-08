using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BAL.Interface
{
    public interface IAdminDashboard
    {
        public bool isMailChanged(string email1, string email2);
        public bool isAdminExistById(int id);
        public bool isAdminExist(string email);
        public string getMailByAdminId(int adminId);    
        public string username(string email);
        public Admin getAdminByemail(string email); 
        public DAL.DataModels.Request reqbyreqid(int id);
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
        public void deleteRequest(int requestId);
        public void unblockRequest(int requestId);
        public string getAspNetIdByPhysicianid(int physicianid);
        public void UpdateProviderProfile(int id, string businessName, string businessWebsite, IFormFile signatureFile, IFormFile photoFile);
    }
}
