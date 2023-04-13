using DadJoke.API.Implementation;
using DadJoke.API.Interface;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Polly;


var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

builder.Services.Configure<CorsOptions>("AllowSpecificOrigins", options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(configuration.GetSection("CorsPolicy:AllowedOrigins").Value?.Split(',')?? new string[0])
               .AllowAnyMethod()
               .AllowAnyHeader();
                
    });
});


// Add services to the container.
builder.Services.AddTransient<HttpClient>();
builder.Services.AddTransient<IJokeProvider, JokeProvider>();


builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    await next();
});

app.MapControllers();

app.Run();
