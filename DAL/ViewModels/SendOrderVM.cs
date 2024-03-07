using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class SendOrderVM
    {
        public List<HealthProfessionalType>? Professions { get; set; }
        public string Vendors { get; set; }
        public string? BusinessContact { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; } 

    }
}
