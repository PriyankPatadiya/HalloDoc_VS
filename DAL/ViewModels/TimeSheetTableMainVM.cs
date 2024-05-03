using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimeSheetTableMainVM
    {
        public bool isFinalize { get; set; }
        public List<TimeSheetTableVM> firstTable { get; set; }
        public List<TimeSheetReimVM> secondTable { get; set; }
    }
}
