

namespace DAL.ViewModels
{
    public class AdminMainPageVM
    {
        public PageName? PageName { get; set; }
       public ViewCaseVM? Casemodel { get; set; }         
        public AdminDashboardVM? DashboardVM { get; set;}

        public ViewNotesVM? NotesVM { get; set; }   

        public ViewdocumentVM? DocumentVM { get; set; }

    }
    public enum PageName
    {
        Dashboard , 
        ViewCaseForm ,
        ViewNotes,
        viewdocument
    }
}
