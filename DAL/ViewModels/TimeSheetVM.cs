using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimeSheetVM
    {
        public DateOnly startdate { get; set; }
        public DateOnly enddate { get; set; }
        public int? physicianId { get; set; }
        public List<TimeSheetForm> forms { get; set; }  
    }
}
