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

public class MinioFileStorage : IFileStorage
{
    private readonly MinioClient _client;
    private readonly string _bucketName;

    private bool _bucketChecked = false;

    public MinioFileStorage(MinioClient minioClient, IOptions<MinioOptions> options) 
    {
        _client = minioClient;
        _bucketName = options.Value.BucketName;
    }

    private async Task EnsureBucketExistsAsync()
    {
        if (_bucketChecked)
            return;

        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucketName)
        );

        if (!exists)
        {
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_bucketName)
            );
        }

        _bucketChecked = true;
    }

    public async Task<Result<string>> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Result<string>.Error("Файл пуст.");
        }

        await EnsureBucketExistsAsync();

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); // Добавьте расширение
        Console.WriteLine($"Попытка загрузить файл: {fileName}");

        try
        {
            using var stream = file.OpenReadStream();

            // Проверьте длину потока
            if (stream.Length == 0)
            {
                return Result<string>.Error("Поток файла пуст.");
            }

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(file.ContentType ?? "application/octet-stream"); // Дефолтный ContentType

            Console.WriteLine("Вызов PutObjectAsync...");
            await _client.PutObjectAsync(putObjectArgs);
            Console.WriteLine("Файл успешно загружен.");

            return Result.Success(fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки файла: {ex}");
            return Result<string>.Error(ex.Message);
        }
    }
    public async Task<Result<Stream>> DownloadFileAsync(string fileName)
    {
        await EnsureBucketExistsAsync();

        var stream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName)
            .WithCallbackStream(async (s, ct) => await s.CopyToAsync(stream));

        await _client.GetObjectAsync(getObjectArgs);
        stream.Position = 0;
        
        return Result.Success<Stream>(stream);
    }

    public async Task<Result> DeleteFileAsync(string fileName)
    {
        await EnsureBucketExistsAsync();

        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileName);

        await _client.RemoveObjectAsync(removeObjectArgs);
        
        return Result.Success();
    }

    public async Task<Result<string>> GetContentAsync(string fileName)
    {
        await EnsureBucketExistsAsync();

        try
        {
            var streamResult = await DownloadFileAsync(fileName);
            if (!streamResult.IsSuccess)
                return Result<string>.Error(string.Join(", ", streamResult.Errors));

            using var reader = new StreamReader(streamResult.Value);
            return Result<string>.Success(await reader.ReadToEndAsync());
        }
        catch (Exception ex)
        {
            return Result<string>.Error(ex.Message);
        }
    }
}
