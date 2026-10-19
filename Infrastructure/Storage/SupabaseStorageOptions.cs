namespace Infrastructure.Storage;

public sealed class SupabaseStorageOptions
{
    public string Endpoint { get; init; } = string.Empty;

    public string Region { get; init; } = string.Empty;

    public string Bucket { get; init; } = string.Empty;
}
