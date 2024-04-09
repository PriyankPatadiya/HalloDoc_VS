using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IAdminActions
    {

        public RequestNote reqnotebyreqid(int requestid);
        public List<RequestClient> clientsbyreqid(int requestid); 
        // Assign Case 
        public void ChangeOnAssign(int reeqid, int phyid, string notes);

        // View Case 
        public IQueryable<ViewCaseVM> getViewCaseData(int reqclientId);
        public bool changeStatusOnCancleCase(int requesid, string? reason, string? Notes);

        public List<Physician> GetPhysicianByRegion(string RegionId);

        // View Notes
        public ViewNotesVM viewnotes(int id);

        // Block Case
        public void BlockCase(int requestId, string reason);
        public List<HealthProfessional> getVenbypro(string ProfessionId);

        // Transfer Notes
        public bool transferNotes(int requestid, int phyid, string transnotes);

        public bool ClearCase(int requestId);

        public void addrequnotes(ViewNotesVM model, RequestNote notes);
        
        // Close Case

        public CloseCaseVM closecasegetdata(CloseCaseVM model, int reqid);

        public bool closecase(CloseCaseVM model);
        public void closeRequest(Request request, int reqid);

        // Encounter form

        public List<EncounterFormVM> getEncounterformdata(int requestid);
        public void addencounterdata(EncounterFormVM model);
        public bool finalize(int requestid);

        // Send Order

        public bool sendOrder(SendOrderVM model);

        public List<HealthProfessionalType> healthProfessionalTypes();
        public List<SchedulingVM> getEvents(int regionId);
    }
}
