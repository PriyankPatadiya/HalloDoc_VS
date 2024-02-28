using DAL.DataModels;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminDashboardVM
    {
        public int? NewCount { get; set; }
        public int? PendingCount { get; set; }
        public int? ActiveCount { get; set; }
        public int? ConcludeCount { get; set; }
        public int? ToCloseCount { get; set; }
        public int? UnpaidCount { get; set; }

        public List<AdminDashboardTableVM> DashboardTableVM { get; set; }

        public List<Region>? Region { get; set; }
    }
}