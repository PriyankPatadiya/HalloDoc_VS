using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using DAL.DataModels;

namespace DAL.ViewModels
{
    public class PatientReqVM
    {

        // For ASPNETUSER

        
        [StringLength(128)] 
        public string? Id { get; set; } = null!;

        [Column(TypeName = "character varying")]
        
        [RegularExpression(@"/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]).{8,32}$/", ErrorMessage = "Invalid Password")]
        public string? PasswordHash { get; set; } = null!;
        
        
        [Column(TypeName = "character varying")]
        [RegularExpression(@"/^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=|\]).{8,32}$/", ErrorMessage = "Invalid Password")]
        public string? ConfirmPasswordHash { get; set; }

        [Key]
        [StringLength(256)]
        [Required(ErrorMessage = " Email Is Required")]
        [DataType(DataType.EmailAddress , ErrorMessage = "Invalid Email Address")]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string Email { get; set; } = null!;

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        [Required(ErrorMessage = " BirthDate Is Required")]
        public DateTime? BirthDate { get; set; } = null!;


        [Required(ErrorMessage = " Patient FirstName Is Required")]
        [StringLength(50, ErrorMessage = "First Name should be between {2} and {1} characters.", MinimumLength = 2)]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        [Required(ErrorMessage = " Patient LastName Is Required")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string LastName { get; set; } = null!;

        [StringLength(100)]
        [Required(ErrorMessage = "Fill This Field")]
        public string? Street { get; set; } 

        [StringLength(100)]
        [Required(ErrorMessage = "Fill This Field")]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter a valid 6-digit zip code")]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? AdminNote { get; set; }
        [StringLength(10)]
        public string? ImageUrl { get; set; }

        [StringLength(120)]
        public string? ImagePath { get; set; }

        public IFormFile? Document { get; set; } 
        public int? Typeid { get; set; }
    
        public List<Region>? Region { get; set; }  

        public int SelectedStateId { get; set; }

        public string? confirmationnumber { get; set; }
    }
}
