using BAL.Interface;
using DAL.DataContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BAL.Repository
{
    public class UploadFileRepo : IuploadFile
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;
        public UploadFileRepo(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _environment = env;
        }
        public void uploadfile(IFormFile file,string filename,  string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

           
            using (FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }        
    }
}
