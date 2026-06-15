using FluentValidation;
using LogicaServidor.BackgroundServices;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Models.Entities;
using LogicaServidor.Repositories;
using LogicaServidor.Services;
using LogicaServidor.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;
using System.Runtime.InteropServices;

bool IsCommandInstalled(string command)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "/bin/sh",
            Arguments = $"-c \"command -v {command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };

    process.Start();

    string output = process.StandardOutput.ReadToEnd().Trim();
    process.WaitForExit();

    return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output);
}
string GetPackageManager()
{
    string[] managers = { "apt", "dnf", "yum", "pacman", "zypper", "apk" };

    string? path = Environment.GetEnvironmentVariable("PATH");

    if (string.IsNullOrWhiteSpace(path))
        return "Gestor desconocido";

    foreach (var manager in managers)
    {
        if (path.Split(Path.PathSeparator)
            .Any(dir => File.Exists(Path.Combine(dir, manager))))
        {
            return manager;
        }
    }

    return "Gestor desconocido";
}
string InstallPackage(string package) /*Regresa el erro en caso de haberlo*/
{
    var process = new Process()
    {
        StartInfo = new()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"sudo {GetPackageManager()} install -y {package}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.Start();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();
    return error;
}
void AskForToInstallPackage(string package)
{
    string r = "";
    while (!(r.ToLower() == "s" || r.ToLower() == "n"))
    {
        Console.WriteLine($"Necesitas tener instalado el paquete {package} instalado en el sistema\n¿Deseas instalarlo? n/s");
        r = Console.ReadLine();
    }
    if (r.ToLower() == "s")
    {
        Console.Clear();
        string error = InstallPackage(package);
        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.WriteLine("Error:");
            Console.WriteLine(error);
        }
        else
        {
            Verification_ipcalc();
        }
    }
}
void Verification_ipcalc()
{
    if (IsCommandInstalled("ipcalc"))
    {
        API();
    }
    else
    {
        AskForToInstallPackage("ipcalc");
    }
}
void Verification_nmap()
{
    if (IsCommandInstalled("nmap"))
    {
        Verification_ipcalc();
    }
    else
    {
        AskForToInstallPackage("nmap");
    }
}
void API()
{
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
}

if (OperatingSystem.IsLinux())
{
    [DllImport("libc")]
    static extern uint geteuid();
    if (geteuid() == 0)
    {
        Verification_nmap();
    }
    else
    {
        Console.WriteLine("Este programa esta diseñado para ejecutarse como usuario privilegiado");
    }
}
else
{
    Console.WriteLine("Este programa esta diseñado para ser ejecutado en una distribucion Linux");
}