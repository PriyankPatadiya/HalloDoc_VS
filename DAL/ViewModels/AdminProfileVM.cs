using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminProfileVM
    {
        public string UserName { get; set; } 
        public string Password { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Email { get; set; }
        public string Confirmemail { get; set; }
        public string phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }    
        public string city { get; set; }
        public int state { get; set; }
        public string zipcode { get; set; }
        public string billingphone { get; set; }
        public List<Region> Region { get; set; }
        public int SelectedStateId { get; set; }
    }
}
