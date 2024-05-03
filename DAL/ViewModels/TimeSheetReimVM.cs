using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class TimeSheetReimVM
    {
        public DateOnly Date { get; set; }
        public string item { get ; set; }
        public int Amount { get; set; }
        public IFormFile? Billfile { get; set; }

    }
}
