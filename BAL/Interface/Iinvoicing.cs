using DAL.ViewModels;

namespace BAL.Interface
{
    public interface Iinvoicing
    {
        public List<TimeSheetForm> getTimesheetdetails(string physicianId, string date);
    }
}
