using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatDashTableVM
    {
       

        public DateTime CreatedDate { get; set; }

        public int currentStatus { get; set; }

        public string? Document { get; set; }

        public int? requestid { get; set; }

        public int count { get; set; }
    }
}
