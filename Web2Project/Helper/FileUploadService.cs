using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web2Project.Helper
{
    public class FileUploadService : IFileUploadService
    {
        IWebHostEnvironment iwebhost;

        public FileUploadService(IWebHostEnvironment iwebhost)
        {
            this.iwebhost = iwebhost;
        }

        public async Task<string> UploadFile(IFormFile ifile, string Id, string folderName)
        {
            string imgext = Path.GetExtension(ifile.FileName).ToLower();
            string saveimg;

            if (imgext == ".png")
            {
                saveimg = Path.Combine(iwebhost.WebRootPath, folderName, Id + ".jpg");
            }
            else
            {
                saveimg = Path.Combine(iwebhost.WebRootPath, folderName, Id + imgext);
            }

            var stream = new FileStream(saveimg, FileMode.Create);
            await ifile.CopyToAsync(stream);

            return saveimg;
        }
    }
}
