using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Storage;
using Domain.Identity;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<Tabi3DbContext>(options => options.UseNpgsql(connectionString));
        }

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddIdentityCore<Tabi3User>().AddRoles<IdentityRole>().AddEntityFrameworkStores<Tabi3DbContext>();

        var storageEndpoint = configuration["Storage:Endpoint"];
        var storageRegion = configuration["Storage:Region"];
        var storageBucket = configuration["Storage:Bucket"];
        var accessKeyId = configuration["S3:AccessKeyId"];
        var secretAccessKey = configuration["S3:SecretAccessKey"];

        var hasStorageConfiguration = new[] { storageEndpoint, storageRegion, storageBucket, accessKeyId, secretAccessKey }
            .Any(value => !string.IsNullOrWhiteSpace(value));

        if (hasStorageConfiguration)
        {
            if (!Uri.TryCreate(storageEndpoint, UriKind.Absolute, out var endpoint) || endpoint.Scheme != Uri.UriSchemeHttps ||
                string.IsNullOrWhiteSpace(storageRegion) || string.IsNullOrWhiteSpace(storageBucket) ||
                string.IsNullOrWhiteSpace(accessKeyId) || string.IsNullOrWhiteSpace(secretAccessKey))
            {
                throw new InvalidOperationException("Supabase Storage configuration is incomplete. Configure Storage:Endpoint, Storage:Region, Storage:Bucket, S3:AccessKeyId, and S3:SecretAccessKey.");
            }

            var storageOptions = new SupabaseStorageOptions
            {
                Endpoint = endpoint.AbsoluteUri.TrimEnd('/'),
                Region = storageRegion,
                Bucket = storageBucket
            };

            services.AddSingleton(storageOptions);
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var clientConfiguration = new AmazonS3Config
                {
                    ServiceURL = storageOptions.Endpoint,
                    AuthenticationRegion = storageOptions.Region,
                    ForcePathStyle = true
                };

                return new AmazonS3Client(
                    new BasicAWSCredentials(accessKeyId, secretAccessKey),
                    clientConfiguration);
            });
            services.AddSingleton<IFileStorage, SupabaseS3FileStorage>();
        }

        return services;
    }
}
