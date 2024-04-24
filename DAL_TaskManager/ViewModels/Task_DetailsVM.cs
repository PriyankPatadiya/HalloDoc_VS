using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_TaskManager.ViewModels
{
    public class Task_DetailsVM
    {
        public int? TaskId { get; set; }
        public string? TaskName { get; set; }
        public string? TaskDescription { get; set; }
        public string? Assignee { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Category { get; set; }
        public string? city { get; set; }
    }
}
