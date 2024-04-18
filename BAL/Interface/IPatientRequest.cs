using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IPatientRequest
    {
        public RequestClient reqclientByRequestId(int requsestId);
        public bool isReqClientExist(int requsestId);
        public List<RequestWiseFile> requestWiseFilesById(int requsestId);
        public Request requestByRequestId(int requsestId);

        void AddPatientForm(PatientReqVM model);
        void Addrequestwisefile(string filename , int requestId);

        public RequestClient GetUserByEmail(string email);

        public Task<string> GetStateAccordingToRegionId(int regionId);

        public string GenerateConfirmationNumber(PatientReqVM model);

        public void AddAdminCreateRequest(PatientReqVM model, string email ,bool isPhysician, int physicianId);
        public void AddRequestForElse(PatientReqVM pInfo);
    }
}
