using DAL.DataModels;
using DAL.ViewModels;

namespace BAL.Interface
{
    public interface IScheduling
    {
        public bool IsShiftExist(SchedulingVM model, int? physicianId);
        public void addShift(SchedulingVM model, int? physicianId, string? creater, Shift shift);
        public Shift getShiftbyPhyId(int physicianId);
        public bool IsRepeatedShiftExist(SchedulingVM model, int? physicianId, DateTime startDateForWeekday, int i);
        public void AddRepeatedShift(SchedulingVM model, int? physicianId, DateTime startDateForWeekday, Shift shift, int i);

        public List<SchedulingVM> getEvents(int regionId);
        public List<SchedulingVM> getProviderEvents(int physicianId);

        public List<Physician> getShiftPhysicians(string region);

        public ShiftDetail sdBysDId(int shiftDetailId);
        public void saveShift(ShiftDetail shiftdetail, DateTime startDate, TimeOnly startTime, TimeOnly endTime);
        public void deleteShift(ShiftDetail shiftdetail);
        public void changeStatus(ShiftDetail shiftdetail);
        public List<ShiftReviewVM> listBYRegion(int regionId);
        public void ApproveShifts(string[] shiftIds);
        public void DeleteShifts(string[] shiftIds);
        public List<Physician> OnDuty(string regionId);
        public void returnShift(ShiftDetail sd);
        public List<Physician> OffDuty(string regionId);
    }
}
