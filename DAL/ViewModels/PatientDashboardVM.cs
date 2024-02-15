using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class PatientDashboardVM
    {
        public DateTime CreatedDate { get; set; }

        public int currentStatus { get; set; }

        public string? Document { get; set; }

        public int? requestid { get; set; }
    }
}
