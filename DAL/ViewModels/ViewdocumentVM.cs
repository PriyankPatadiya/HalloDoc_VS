using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ViewdocumentVM
    { 
        public List<RequestWiseFile>? RequestWiseFile { get; set; }    

        public int? requestid { get; set; }

        public string? confirmationNumber { get; set; }

    }
}
