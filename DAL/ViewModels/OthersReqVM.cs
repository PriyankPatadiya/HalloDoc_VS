using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.ViewModels
{
    public class OthersReqVM
    {
       
        // For Other Three Request Forms ( FamFrnd , Concierge , Business )

        [Key]
        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter First Name")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string YourFirstName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter Last Name")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string YourLastName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter  Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string YourEmail { get; set; } = null!;

        [StringLength(256)]
        public string? Relation { get; set; }

        [StringLength(256)]
        public string? BusinessName { get; set; }

        [StringLength(256)]
        public string? HotelName { get; set; }

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = " Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]

        public string? PhoneNumber { get; set; }

        [Column(TypeName = "character varying")]
        [Required(ErrorMessage = "Your Phone number Is Required")]
        [RegularExpression(@"^(?:(\+?91|0)?[ ]?([\-\s]?[6-9]\d{9})|(\+?91|0)?[ ]?(\d{5})[ ]?(\d{5}))$", ErrorMessage = "Enter a valid Phone number")]

        public string? YourPhoneNumber { get; set; } 

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter FirstName Of Patient")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string FirstName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter LastName Of Patient")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string LastName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter The Email Of Patient")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]

        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter The DOB Of the Patient")]

        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; } = null!;
          public string? State { get; set; } 

        [Required(ErrorMessage = "Please Enter Zipcode")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip code must be 5 digits")]

        public string Zipcode { get; set; } = null!;

        
        public IFormFile? Document { get; set; }

        public List<Region>? Region { get; set; }

        public int SelectedStateId { get; set; }

        public string? confirmationnumber { get; set; }

    }

}

