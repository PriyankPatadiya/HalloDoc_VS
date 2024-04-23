using DAL.DataModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ViewModels
{
    public class ViewCaseVM
    {
        public int? RequestClientId { get; set; }

        public int? Status { get; set; }
        public string? ConfirmationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = " Email Is Required")]
        public string FirstName { get; set; } = null!;
        
        [Required(ErrorMessage = " Email Is Required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = " Email Is Required")]
        public string? Notes { get; set; }

        [StringLength(256)]
        [Required(ErrorMessage = " Email Is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string? Email { get; set; }

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? Phone { get; set; }

        public string address { get; set; } = null!;

        public string? region { get; set; }

        public int? requestid { get; set; }
        public bool isPhysician { get; set; }

        public List<Region>? regiontable { get; set; }
    }
}
