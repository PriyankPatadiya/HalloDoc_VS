using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
