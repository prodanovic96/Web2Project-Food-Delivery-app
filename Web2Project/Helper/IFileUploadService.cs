using Microsoft.AspNetCore.Http;

namespace Web2Project.Helper
{
    public interface IFileUploadService
    {
        string UploadFile(IFormFile ifile, string Id);
        string ReturnUnknownUser();
    }
}
