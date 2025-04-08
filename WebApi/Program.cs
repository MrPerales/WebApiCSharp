using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// variable para cadena de conection 
var connectionString = builder.Configuration.GetConnectionString("Connection");

// registrar servicio para la conexion 
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//agregamos todos los validators en el PROGRAM 
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// las validaciones se hacen manualmente , por eso agregamos la sig linea para que sea automaticas 
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
