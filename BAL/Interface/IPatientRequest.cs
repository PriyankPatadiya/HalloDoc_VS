using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IPatientRequest
    {
        void AddPatientForm(PatientReqVM model);
        void Addrequestwisefile(string filename , int requestId);

        public RequestClient GetUserByEmail(string email);

        public Task<string> GetStateAccordingToRegionId(int regionId);

        public string GenerateConfirmationNumber(PatientReqVM model);
    }
}
