using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Storage;

namespace Infrastructure.Storage;

public sealed class SupabaseS3FileStorage(IAmazonS3 s3Client, SupabaseStorageOptions options) : IFileStorage
{
    private static readonly TimeSpan MaximumUrlLifetime = TimeSpan.FromMinutes(15);

    public Uri CreateUploadUrl(string objectKey, string contentType, TimeSpan lifetime)
    {
        ValidateObjectKey(objectKey);

        if (string.IsNullOrWhiteSpace(contentType) || contentType.Length > 255 || contentType.Any(char.IsControl))
            throw new ArgumentException("A valid content type is required.", nameof(contentType));

        ValidateLifetime(lifetime);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(lifetime)
        };

        return new Uri(s3Client.GetPreSignedURL(request), UriKind.Absolute);
    }

    public Uri CreateDownloadUrl(string objectKey, TimeSpan lifetime)
    {
        ValidateObjectKey(objectKey);
        ValidateLifetime(lifetime);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(lifetime)
        };

        return new Uri(s3Client.GetPreSignedURL(request), UriKind.Absolute);
    }

    private static void ValidateObjectKey(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey) || objectKey.StartsWith('/') || objectKey.Contains('\\') || objectKey.Any(char.IsControl))
            throw new ArgumentException("A valid storage object key is required.", nameof(objectKey));

        if (objectKey.Split('/').Any(segment => segment is "." or ".."))
            throw new ArgumentException("Storage object keys cannot contain relative path segments.", nameof(objectKey));
    }

    private static void ValidateLifetime(TimeSpan lifetime)
    {
        if (lifetime <= TimeSpan.Zero || lifetime > MaximumUrlLifetime)
            throw new ArgumentOutOfRangeException(nameof(lifetime), $"Signed URL lifetime must be greater than zero and no more than {MaximumUrlLifetime.TotalMinutes} minutes.");
    }
}
