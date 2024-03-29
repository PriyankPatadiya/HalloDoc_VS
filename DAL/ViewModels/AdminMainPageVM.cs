﻿

namespace DAL.ViewModels
{
    public class AdminMainPageVM
    {
        public PageName? PageName { get; set; }
        public ViewCaseVM? Casemodel { get; set; }         
        public AdminDashboardVM? DashboardVM { get; set;}
        public ViewNotesVM? NotesVM { get; set; }   

        public ViewdocumentVM? DocumentVM { get; set; }
        public SendOrderVM? SendOrderVM { get; set; }
        public CloseCaseVM? closecase { get; set; }
        public EncounterFormVM? encountermodel { get; set; }
 
    }
    public enum PageName
    {
        Dashboard , 
        ViewCaseForm ,
        ViewNotes,
        viewdocument,
        SendOrder,
        CloseCase,
        Encounterform
    }
}
