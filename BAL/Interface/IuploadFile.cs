using Microsoft.AspNetCore.Http;

namespace BAL.Interface
{
    public interface IuploadFile
    {
        void uploadfile(IFormFile file,string filename, string path);
    }
}
