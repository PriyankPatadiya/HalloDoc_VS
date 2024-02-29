using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminMainPageVM
    {
        public PageName? PageName { get; set; }
       public ViewCaseVM? Casemodel { get; set; }         
        public AdminDashboardVM? DashboardVM { get; set;}

        public ViewNotesVM? NotesVM { get; set; }   

    }
    public enum PageName
    {
        Dashboard , 
        ViewCaseForm ,
        ViewNotes
    }
}
