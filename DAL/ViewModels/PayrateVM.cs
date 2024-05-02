using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PayrateVM
    {
        public int? PayrateId { get; set; }
        public int? PayrateCategoryId { get; set; }
        public string? PayrateCategoryName { get; set; }
        public int? Physicianid { get; set; }
        public decimal? Payrate { get; set; }
    }
}
