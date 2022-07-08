using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Web2Project.Helper
{
    public class FileUploadService : IFileUploadService
    {
        IWebHostEnvironment iwebhost;

        public FileUploadService(IWebHostEnvironment iwebhost)
        {
            this.iwebhost = iwebhost;
        }

        public string ReturnUnknownUser()
        {
            var saveimg = Path.Combine(iwebhost.WebRootPath, "Images" + "web2.jpg");
            return saveimg;
        }

        public string UploadFile(IFormFile ifile, string Id)
        {
            string ext = Path.GetExtension(ifile.FileName).ToLower();

            var saveimg = Path.Combine(iwebhost.WebRootPath, "Images", Id + ext);
            var stream = new FileStream(saveimg, FileMode.Create);
            ifile.CopyToAsync(stream);

            return saveimg;
        }
    }
}
