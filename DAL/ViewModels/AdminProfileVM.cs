using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ViewModels
{
    public class AdminProfileVM
    {
        public string? UserName { get; set; }

        [Column(TypeName = "character varying")]
        [RegularExpression(@"/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]).{8,32}$/", ErrorMessage = "Invalid Password")]
        public string Password { get; set; } = null!;
        public string? Status { get; set; }
        public string? Role { get; set; }

        [Required(ErrorMessage = " FirstName Is Required")]
        [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string? FirstName { get; set; } = null!;

        [Required(ErrorMessage = " FirstName Is Required")]
        [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string LastName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = " Email Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string Email { get; set; } = null!;
        public string Confirmemail { get; set; } = null!;

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? phone { get; set; } 
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }    
        public string? city { get; set; }
        public int state { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zip code")]
        public string? zipcode { get; set; }

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? billingphone { get; set; } = null!;
        public List<Region>? Region { get; set; }
        public int SelectedStateId { get; set; }
        public List<CheckboxList_model>? statess { get; set; }
    }
    public class CheckboxList_model
    {
        public int? Value { get; set; }
        public Boolean? Selected { get; set; }
    }
}
