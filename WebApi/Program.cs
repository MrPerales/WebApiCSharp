using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using WebApi.Services;
using WebApi.Models.Common;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddScoped<IUserService, UserService>(); //injection
builder.Services.AddScoped<IVentaService,VentaService>(); //si el service utiliza una Interfaz ponerla asi y tambien en el constructor del controlador 


//config JWT
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

//jwt 
var appSettings= appSettingsSection.Get<AppSettings>();//
var key = Encoding.ASCII.GetBytes(appSettings.Secreto); //arreglo de bits del secreto 
//damos de alta el token 
builder.Services.AddAuthentication(d =>
{
    d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(d =>
{
    d.RequireHttpsMetadata = false;
    d.SaveToken = true;  //vida del token
    d.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key), //aqui va la llave 
        ValidateIssuer= false,
        ValidateAudience= false,
    };

});
//cors config
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MiCors",
                        builder =>
                        {
                            builder.WithOrigins("*");
                            builder.WithHeaders("*");
                            builder.WithMethods("*");
                        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("MiCors");
app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
