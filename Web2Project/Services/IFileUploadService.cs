using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web2Project.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFile(IFormFile ifile, string Id, string folderName);
    }
}
