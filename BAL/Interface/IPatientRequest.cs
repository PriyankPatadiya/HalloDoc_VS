using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IPatientRequest
    {
        void AddPatientForm(PatientReqVM model);
        void Addrequestwisefile(string filename , int requestId);

        public RequestClient GetUserByEmail(string email);

        public Task<string> GetStateAccordingToRegionId(int regionId);

        public string GenerateConfirmationNumber(PatientReqVM model);

        public void AddAdminCreateRequest(PatientReqVM model, string email);
    }
}
