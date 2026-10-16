using Supabase.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Registers Supabase.Client and each of its sub-clients (Auth, Postgrest,
// Storage, Functions, Realtime) in the DI container, backed by pooled
// HttpClients from IHttpClientFactory.
//
// Blazor registers the client the same way: use this in your Blazor Server /
// Web App Program.cs, or WebAssemblyHostBuilder.CreateDefault(args) for a
// Blazor WebAssembly client.
builder.Services.AddSupabase(
    builder.Configuration["Supabase:Url"]!,
    builder.Configuration["Supabase:Key"]!
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/todos", async (Supabase.Client supabase) =>
{
        await supabase.InitializeAsync();
        var response = await supabase.From<Todo>().Get();
        return response.Models;
});

app.Run();
