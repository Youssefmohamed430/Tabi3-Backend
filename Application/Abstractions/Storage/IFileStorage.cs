namespace Application.Abstractions.Storage;

public interface IFileStorage
{
    Uri CreateUploadUrl(string objectKey, string contentType, TimeSpan lifetime);

    Uri CreateDownloadUrl(string objectKey, TimeSpan lifetime);
}
