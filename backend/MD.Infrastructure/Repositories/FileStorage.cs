using Ardalis.Result;
using MD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MD.Infrastructure;

public class MinioOptions
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
}

public class FileStorage : IFileStorage
{
    private readonly MinioClient _client;
    private readonly string _bucketName;

    public FileStorage(MinioClient minioClient, IOptions<MinioOptions> options) 
    {
        _client = minioClient;
        _bucketName = options.Value.BucketName;
    }
    public async Task<Result<string>> UploadFileAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        using var stream = file.OpenReadStream();

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(file.ContentType);

        await _client.PutObjectAsync(putObjectArgs);
        
        return Result.Success(fileName);
    }
    public async Task<Result<Stream>> DownloadFileAsync(string fileName)
    {
        var stream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(fileName)
            .WithObject(fileName)
            .WithCallbackStream(async (s, ct) => await s.CopyToAsync(stream));

        await _client.GetObjectAsync(getObjectArgs);
        stream.Position = 0;
        
        return Result.Success<Stream>(stream);
    }

    public async Task<Result> DeleteFileAsync(string fileName)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName);

        await _client.RemoveObjectAsync(removeObjectArgs);
        
        return Result.Success();
    }
}
