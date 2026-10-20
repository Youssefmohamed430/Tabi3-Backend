using Infrastructure;
using Infrastructure.Identity;
using Tabi3_Backend.Configuration.Logging;
using Tabi3_Backend.Configuration.RateLimiting;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);
LoadDotEnv(Path.Combine(builder.Environment.ContentRootPath, ".env"));
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddTabi3Serilog(builder.Configuration, builder.Environment)
    .AddTabi3RateLimiting();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (!string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("DefaultConnection")))
    await app.Services.EnsureSuperAdminAsync(builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.Use(async (context, next) =>
{
    using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        await next();
});
app.UseAuthorization();
app.MapControllers();

app.Run();

static void LoadDotEnv(string path)
{
    if (!File.Exists(path))
        return;

    foreach (var line in File.ReadLines(path))
    {
        var entry = line.Trim();
        if (entry.Length == 0 || entry.StartsWith('#'))
            continue;

        var separatorIndex = entry.IndexOf('=');
        if (separatorIndex <= 0)
            continue;

        var key = entry[..separatorIndex].Trim();
        var value = entry[(separatorIndex + 1)..].Trim();
        if (value.Length >= 2 && value[0] == value[^1] && value[0] is '\'' or '"')
            value = value[1..^1];

        if (Environment.GetEnvironmentVariable(key) is null)
            Environment.SetEnvironmentVariable(key, value);
    }
}
