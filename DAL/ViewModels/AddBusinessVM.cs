using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AddBusinessVM
    {
        public string BusinessName { get; set; }
        public int vendorId { get; set; }
        public int? SelecteProfession { get; set; }
        public string? FaxNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? BusinessContact { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }    
        public int? StateId { get; set; }
        public string? ZipCode { get; set; }
    }
}
