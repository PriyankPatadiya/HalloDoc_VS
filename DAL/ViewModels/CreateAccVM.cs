using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels
{
    public class CreateAccVM
    {
        [StringLength(256)]
        public string? UserName { get; set; }

        [Column(TypeName = "character varying")]
        [Required]
        public string? Password { get; set; }

        [Column(TypeName = "character varying")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
