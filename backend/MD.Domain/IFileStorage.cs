using Ardalis.Result;
using Microsoft.AspNetCore.Http;

namespace MD.Domain;

public interface IFileStorage
{
    Task<Result<string>> UploadFileAsync(IFormFile file);
    Task<Result<Stream>> DownloadFileAsync(string fileName);
    Task<Result> DeleteFileAsync(string fileName);

    Task<Result<string>> GetContentAsync(string fileName);
}
