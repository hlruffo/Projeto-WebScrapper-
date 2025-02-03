using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebScrapper.Application.Extensions; // Inclui a ApiServiceCollectionExtensions
using WebScrapper.Infrastructure.Extensions;
using WebScraper.Infra.Data.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Carrega as configurações (appsettings.json)
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Registra os serviços
builder.Services.AddHttpClient();

// Registra os serviços da camada Application (usando a extensão ApiServiceCollectionExtensions)
WebScrapper.Application.Extensions.ApiServiceCollectionExtensions.AddApplicationServices(builder.Services);

// Registra os serviços da camada Infrastructure, utilizando a connection string definida no appsettings.json
builder.Services.AddInfrastructureServices(builder.Configuration);

// Registra os controllers e o Swagger para documentação
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
