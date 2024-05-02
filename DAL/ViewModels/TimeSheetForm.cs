using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimeSheetForm
    {
        public int physicianId { get; set; }
        public DateOnly date { get; set; }
        public int onCallhours { get; set; }
        public decimal? totalHours { get; set; }
        public bool? isWeekend { get; set; } = false;
        public int? HouseCallNo { get; set; }
        public int? PhoneCallNo { get; set; }

    }
}
