using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required , EmailAddress]
        public string email { get; set; }

        public bool? EmailSent { get; set; } 
    }
}
