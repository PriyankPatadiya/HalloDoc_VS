﻿

namespace DAL.ViewModels
{
    public class ShiftReviewVM
    {
        public int status { get; set; }
        public string? physicianName { get; set; }
        public int shiftDetailId { get; set; }

        public string? Regioname { get; set; }

        public string? day { get; set; }

        public TimeOnly? startTime { get; set; }
        public TimeOnly? endTime { get; set;}

    }
}
