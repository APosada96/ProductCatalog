using ProductCatalog.Api;
using ProductCatalog.Api.Endpoints;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Application.Mapping;
using ProductCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiLayer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(o =>
{
    o.AddPolicy("wasm", p => p
        .WithOrigins("https://localhost:7169")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

app.UseApiExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("wasm");
app.MapProductEndpoints();

app.Run();

