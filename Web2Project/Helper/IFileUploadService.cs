using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web2Project.Helper
{
    public interface IFileUploadService
    {
        Task<string> UploadFile(IFormFile ifile, string Id);
    }
}
