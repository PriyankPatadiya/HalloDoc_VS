using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class LoginVM
    {
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [Column(TypeName = "character varying")]
        [Required]
        public string? PasswordHash { get; set; }
    }
}
