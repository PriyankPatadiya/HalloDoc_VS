using DAL.DataModels;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class SendOrderVM
    {
        public List<HealthProfessionalType>? Professions { get; set; }
        [Required(ErrorMessage = "Required")]
        public string profession { get; set; } = null!;
        public int vendorid { get; set; }
        public int requestid { get; set; }
        public string prescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? createdby { get; set; }
        public string? Vendors { get; set; }
        [Required(ErrorMessage = "Required")]
        public string BusinessContact { get; set; } = null!;
        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter  Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Field Is Required")]
        //[RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Invalid Fax Number")]
        public string? Fax { get; set; } 
        public int Noofretail { get; set; }

    }
}
