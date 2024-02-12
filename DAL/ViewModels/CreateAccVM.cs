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
    public class CreateAccVM
    {
        [StringLength(256)]
        public string UserName { get; set; } = null!;

        [Column(TypeName = "character varying")]
        [Required]
        public string? Password { get; set; }

        [Column(TypeName = "character varying")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
