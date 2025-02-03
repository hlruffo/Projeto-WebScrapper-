using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebScrapper.Application.Extensions; 
using WebScrapper.Infrastructure.Extensions;
using WebScraper.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddHttpClient();

WebScrapper.Application.Extensions.ApiServiceCollectionExtensions.AddApplicationServices(builder.Services);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
