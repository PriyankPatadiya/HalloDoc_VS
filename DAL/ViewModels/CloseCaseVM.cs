using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ViewModels
{
    public class CloseCaseVM
    {
        [Key]
        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter First Name")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string FirstName { get; set; } = null!;

        [Key]
        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter Last Name")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string? LastName { get; set; }

        
            public string? ConfirmationNum { get; set; }

            public DateOnly DateOfBirth { get; set; }
            public int? requestid { get; set; } 

            public List<RequestWiseFile> Files { get; set; }
        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter  Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string? Email { get; set; }

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? Phonenum { get; set; }
        
    }
}
