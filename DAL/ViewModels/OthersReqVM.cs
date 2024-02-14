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
        public string YourFirstName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter Last Name")]
        public string YourLastName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter  Email")]
        public string YourEmail { get; set; } = null!;

        [StringLength(256)]
        public string? Relation { get; set; }

        [StringLength(256)]
        public string? BusinessName { get; set; }

        [StringLength(256)]
        public string? HotelName { get; set; }

        [Column(TypeName = "character varying")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "character varying")]
        public string? YourPhoneNumber { get; set; }

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter FirstName Of Patient")]
        public string FirstName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter LastName Of Patient")]
        public string LastName { get; set; } = null!;

        [StringLength(256)]
        [Required(ErrorMessage = "Please Enter The Email Of Patient")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter The DOB Of the Patient")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Please Enter Street")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter State")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Zipcode")]
        public string Zipcode { get; set; } = null!;

        [Required]
        public IFormFile Document { get; set; }


    }

}

