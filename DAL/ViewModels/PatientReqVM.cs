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
    public class PatientReqVM
    {

        // For ASPNETUSER

        
        [StringLength(128)]
        public string? Id { get; set; } 

        [Column(TypeName = "character varying")]
        public string? PasswordHash { get; set; }

        [Key]
        [StringLength(256)]
        public string? Email { get; set; }

        [Column(TypeName = "character varying")]
        public string? PhoneNumber { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }

        public int Typeid { get; set; }
    }
}
