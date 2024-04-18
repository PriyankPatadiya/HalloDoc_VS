using DAL.DataModels;
using DAL.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class SchedulingVM
    {
        public int? Shiftid { get; set; }
        public int? ShiftDetailId { get; set; }

        public int Physicianid { get; set; }
        public string? PhysicianName { get; set; }
        public string? PhysicianPhoto { get; set; }
        public int? Regionid { get; set; }
        public string? RegionName { get; set; }

        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }   
        public DateTime? Shiftdate { get; set; }
        [CustomValidation(typeof(TimeValidation), "ValidateTime")]
        public TimeOnly Starttime { get; set; }

        [CustomValidation(typeof(TimeValidation), "ValidateTime")]
        public TimeOnly Endtime { get; set; }

        public bool Isrepeat { get; set; }

        public string? checkWeekday { get; set; }
        public string? title { get; set; }
        public bool ShiftDeleted { get; set; }
        public int? Repeatupto { get; set; }
        public short Status { get; set; }
        public List<SchedulingVM>? DayList { get; set; }
        public List<Region>? Region { get; set; }
    }
}

public class TimeValidation
{
    public static ValidationResult ValidateTime(TimeOnly time, ValidationContext context)
    {
        var viewModel = context.ObjectInstance as SchedulingVM;
        if (viewModel.Starttime > viewModel.Endtime)
        {
            return new ValidationResult("Start time cannot be later than end time.");
        }
        return ValidationResult.Success;
    }
}