using DAL.ViewModels;

namespace BAL.Interface
{
    public interface Iinvoicing
    {
        public List<TimeSheetForm> getTimesheetdetails(string physicianId, string date);
        public bool isTimeSheetExist(DateOnly startdate);
        public void AddNewSheet(DateOnly date, string physicianId);
        public TimeSheetTableMainVM getTimesheetTableData(string date, int physicianId);
    }
}
