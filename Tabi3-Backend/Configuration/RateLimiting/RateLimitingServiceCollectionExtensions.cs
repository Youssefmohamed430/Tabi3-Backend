using System.Globalization;
using System.Threading.RateLimiting;

namespace Tabi3_Backend.Configuration.RateLimiting;

public static class RateLimitingServiceCollectionExtensions
{
    public static IServiceCollection AddTabi3RateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        Math.Ceiling(retryAfter.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Retry after {retryAfter.TotalSeconds:0.##} seconds.",
                        cancellationToken);
                    return;
                }

                await context.HttpContext.Response.WriteAsync(
                    "Too many requests. Please try again later.",
                    cancellationToken);
            };

            options.AddPolicy("AuthPolicy", httpContext => CreateIpPolicy(
                httpContext,
                permitLimit: 5,
                window: TimeSpan.FromMinutes(1),
                queueLimit: 0));

            options.AddPolicy("GeneralPolicy", httpContext => CreateIpPolicy(
                httpContext,
                permitLimit: 30,
                window: TimeSpan.FromMinutes(1),
                queueLimit: 5));

            //options.AddPolicy("PaymentPolicy", httpContext => CreateIpPolicy(
            //    httpContext,
            //    permitLimit: 3,
            //    window: TimeSpan.FromMinutes(5),
            //    queueLimit: 0));

            // Apply the general policy to all endpoints unless an endpoint selects a stricter policy.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                CreateIpPolicy(httpContext, permitLimit: 30, window: TimeSpan.FromMinutes(1), queueLimit: 5));
        });

        return services;
    }

    private static RateLimitPartition<string> CreateIpPolicy(
        HttpContext httpContext,
        int permitLimit,
        TimeSpan window,
        int queueLimit)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-client";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = queueLimit,
                AutoReplenishment = true
            });
    }
}
