using FluentValidation;
using LogicaServidor.BackgroundServices;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Models.Entities;
using LogicaServidor.Repositories;
using LogicaServidor.Services;
using LogicaServidor.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<SensoresTrinityContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));

builder.Services.AddScoped<DatosService>();
builder.Services.AddScoped<NmapService>();

/*Se agrega como singleton porque usa una lista que si almacena datos en memoria*/
builder.Services.AddSingleton<DispositivosService>();

/*BackgroundServices*/
builder.Services.AddHostedService<LimpiezaRegistroService>();
builder.Services.AddHostedService<InyeccionDatosPlcService>();
builder.Services.AddHostedService<AnunciosRedLocalServices>();

/*Validadores*/
builder.Services.AddScoped<IValidator<DatosDTO>, DatosValidator>();

var app = builder.Build();

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();