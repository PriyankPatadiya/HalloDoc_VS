using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_TaskManager.ViewModels
{
    public class Task_DetailsVM
    {
        public int? TaskId { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public string? TaskName { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public string? TaskDescription { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public string? Assignee { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public DateTime? DueDate { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public string? Category { get; set; }
        [Required(ErrorMessage = "This field is Required!")]

        public string? city { get; set; }
    }
}
