

namespace DAL.ViewModels
{
    public class TimeSheetTableVM
    {
        public DateOnly shiftdate { get; set; }
        public int shiftCount { get; set; }
        public int NightShiftWeekend { get; set; }
        public int HouseCallNightSWeekend { get; set; }
        public int PhoneConsults { get; set; }
        public int PhoneConsultsNightWeekend { get; set; }
        public int BatchTesting { get; set; }

    }
}
