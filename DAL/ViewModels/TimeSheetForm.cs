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
        public int onCallhours { get; set; } = 0;
        public decimal? totalHours { get; set; } = 0;
        public bool isWeekend { get; set; } = false;
        public int? HouseCallNo { get; set; } = 0;
        public int? PhoneCallNo { get; set; } = 0;

    }
}
