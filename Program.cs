using FluentValidation;
using LogicaServidor.BackgroundServices;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Models.Entities;
using LogicaServidor.Repositories;
using LogicaServidor.Services;
using LogicaServidor.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddDbContext<SensoresTrinityContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

builder.Services.AddScoped<DatosService>();

/*Se agrega como singleton porque usa una lista que si almacena datos en memoria*/
builder.Services.AddSingleton<DispositivosService>();
builder.Services.AddSingleton<NmapService>();
builder.Services.AddSingleton<RelacionPlcAndSensorService>();

/*BackgroundServices*/
builder.Services.AddHostedService<ActualizarPlcsService>();
builder.Services.AddHostedService<LimpiezaRegistroService>();
builder.Services.AddHostedService<InyeccionDatosPlcService>();
builder.Services.AddHostedService<AnunciosRedLocalServices>();

/*Validadores*/
builder.Services.AddScoped<IValidator<DatosDTO>, DatosValidator>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Relation}/{action=Relation}");
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Account}/{action=Login}/{id?}");
//app.UseHttpsRedirection();

app.MapControllers();

app.Run();