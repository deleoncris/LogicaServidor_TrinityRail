using LogicaServidor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostedService<ServicioLoop>();
builder.WebHost.UseUrls("http://0.0.0.0:5221");

var app = builder.Build();

app.MapControllers();

app.Run();