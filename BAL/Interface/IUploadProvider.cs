using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IUploadProvider
    {
        public string UploadSignature(IFormFile Sign, int physicianid);
        public string UploadPhoto(IFormFile PhotoName, int physicianid);
        public string UploadDocFile(IFormFile PhotoName, int physicianid, string FileName);
    }
}
