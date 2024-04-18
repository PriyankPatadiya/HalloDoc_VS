using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IRequests
    {
        
        void AddFamilyFriendForm(OthersReqVM model);
        void AddConciergeForm(OthersReqVM model);
        void AddBusinessForm(OthersReqVM model);
        public string GenerateConfirmationNumber(OthersReqVM model);
        public void AggrementReview(Request request, int requestid);
        public void cancelAgreement(int requestid, string PatientNote, Request request);
        public PatientDashboardVM getPatientDashboardData(string email);

        public void EditPatientProfile(ProfilePatient profilepatient);
    }
}
