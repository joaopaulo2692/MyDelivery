using Microsoft.OpenApi.Models;
using MyDelivery.Application.Interfaces;
using MyDelivery.Application.Service;
using MyDelivery.Infrastructure.Repositories;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IOcorrenciaRepository, OcorrenciaRepository>();

// Serilog já registrado com UseSerilog()

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyDelivery API", Version = "v1" });
    // adicionar segurança JWT se implementar auth
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
