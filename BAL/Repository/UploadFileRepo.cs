﻿using BAL.Interface;
using DAL.DataContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class UploadFileRepo : IuploadFile
    {
        private readonly ApplicationDbContext _context;
        public UploadFileRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public void uploadfile(IFormFile file, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var uniquefilesavetoken = new Guid().ToString();

            string fileName = Path.GetFileName(file.FileName);
            fileName = $"{fileName}_{uniquefilesavetoken}";
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

    }
}
