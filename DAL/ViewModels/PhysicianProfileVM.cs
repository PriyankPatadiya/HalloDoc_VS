using System.ComponentModel.DataAnnotations;
using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PhysicianProfileVM
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$",
            ErrorMessage = "Password must be 8-15 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string? Password { get; set; }

        public string? Status { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [RegularExpression(@"^[A-Z][a-z-']*\s*([A-Z][a-z-']*\s*)*$", ErrorMessage = "Invalid Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|gov\.in)$", ErrorMessage = "Enter a valid email address with valid domain")]
        public string? Email { get; set; }

        [Compare("Email", ErrorMessage = "Email and confirm email must match")]
        public string? ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Required!")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string? MobileNo { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public string? Street { get; set; }

        public int? State { get; set; }

        [RegularExpression(@"^\d{5}$", ErrorMessage = "Zip code must be 5 digits")]
        public string? ZipCode { get; set; }

        public List<Region>? Regions { get; set; }

        public List<PhysicianRegion>? WorkingRegions { get; set; }

        public int physicianid { get; set; }

        public string? MedicalLicense { get; set; }

        public string? NPINumber { get; set; }

        public string? SynchronizationEmail { get; set; }

        [Required(ErrorMessage = "BusinessName is Required!")]
        public string? BusinessName { get; set; }

        [Required(ErrorMessage = "BusinessWebsite is required!")]
        public string? BusinessWebsite { get; set; }

        public string? AdminNotes { get; set; }

        public IFormFile? File { get; set; }

        public string? PhotoFileName { get; set; }

        public string? SignatureFilename { get; set; }

        public BitArray? IsAgreement { get; set; }

        public BitArray? IsBackground { get; set; }

        public BitArray? IsHippa { get; set; }

        public BitArray? NonDiscoluser { get; set; }

        public BitArray? License { get; set; }

        public IFormFile? ICAFile { get; set; }

        public IFormFile? BackFile { get; set; }

        public IFormFile? HippaFile { get; set; }

        public IFormFile? NonFile { get; set; }

        public bool? isFromUserAccess { get; set; }
    }
}
