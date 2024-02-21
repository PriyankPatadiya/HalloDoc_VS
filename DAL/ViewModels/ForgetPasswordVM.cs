using System.ComponentModel.DataAnnotations;


namespace DAL.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required, EmailAddress]
        public string email { get; set; }

        public bool? EmailSent { get; set; }

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}