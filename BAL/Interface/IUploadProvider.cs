using Microsoft.AspNetCore.Http;

namespace BAL.Interface
{
    public interface IUploadProvider
    {
        public string UploadSignature(IFormFile Sign, int physicianid);
        public string UploadPhoto(IFormFile PhotoName, int physicianid);
        public void UploadDocFile(IFormFile PhotoName, int physicianid, string FileName);
    }
}
