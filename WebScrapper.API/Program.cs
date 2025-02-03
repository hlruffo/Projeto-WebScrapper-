using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebScrapper.Application.Extensions; // Inclui a ApiServiceCollectionExtensions
using WebScrapper.Infrastructure.Extensions;
using WebScraper.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Carrega as configura��es (appsettings.json)
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Registra os servi�os
builder.Services.AddHttpClient();

// Registra os servi�os da camada Application (usando a extens�o ApiServiceCollectionExtensions)
WebScrapper.Application.Extensions.ApiServiceCollectionExtensions.AddApplicationServices(builder.Services);

// Registra os servi�os da camada Infrastructure, utilizando a connection string definida no appsettings.json
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registra os controllers e o Swagger para documenta��o
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura��o do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
