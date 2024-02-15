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
        public string uploader { get; set; }
        public DateTime UploadDate { get; set; }    
        public string? title { get; set; }
    }
}
