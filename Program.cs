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
using System.Net.Sockets;
using System.Runtime.InteropServices;
using MySqlConnector;

/*Esta shit necesita realmente optimizacion del flujo de all, se ve horrible */
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
bool IsPortOpen()
{
    try
    {
        using var client = new TcpClient();
        var result = client.BeginConnect("127.0.0.1", 3306, null, null);
        bool success = result.AsyncWaitHandle.WaitOne(1500);
        if (!success) return false;

        client.EndConnect(result);
        return true;
    }
    catch
    {
        return false;
    }
}
bool CanConnectDatabase()
{
    try
    {
        var connStr = $"Server=127.0.0.1;Port=3306;User ID=dbeaver;Password=root;";
        using var conn = new MySqlConnection(connStr);
        conn.Open();
        return true;
    }
    catch
    {
        return false;
    }
}
void InstallAndStartMariaDB()
{
    RunShell("apt update");
    RunShell("apt install mariadb-server -y");
    RunShell("systemctl enable mariadb");
    RunShell("systemctl start mariadb");
}
void SetupDatabaseUser()
{
    string sql = "CREATE USER IF NOT EXISTS 'dbeaver'@'localhost' IDENTIFIED BY 'root'; " +
                 "GRANT ALL PRIVILEGES ON *.* TO 'dbeaver'@'localhost' WITH GRANT OPTION; " +
                 "FLUSH PRIVILEGES;";

    string cmd = $"mysql -e \"{sql}\"";
    RunShell(cmd);
}
void RunShell(string command)
{
    var process = new Process();
    process.StartInfo.FileName = "/bin/bash";
    process.StartInfo.Arguments = $"-c \"{command}\"";
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;

    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();

    Console.WriteLine(output);
    if (!string.IsNullOrEmpty(error))
        Console.WriteLine("ERROR: " + error);
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
    /*Crea bd en caso de no existir*/
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SensoresTrinityContext>();
        db.Database.EnsureCreated();
    }
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
void VerificarExistenciaUsuario()
{
    if (!CanConnectDatabase())
    {
        string r = "";
        while (!(r.ToLower() == "s" || r.ToLower() == "n"))
        {
            Console.WriteLine("Se requiere de un usuario especial en MariaDB\n¿Deseas crear al usuario 'dbeaver' con contraseña 'root'? n/s");
            r = Console.ReadLine();
        }
        if (r.ToLower() == "s")
        {
            Console.Clear();
            SetupDatabaseUser();
            VerificacionModoPrivilegiado();
        }
    }
    else
    {
        VerificacionModoPrivilegiado();
    }
}
void VerificacionModoPrivilegiado()
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
if (OperatingSystem.IsLinux())
{
    if (!IsPortOpen())
    {
        string r = "";
        while (!(r.ToLower() == "s" || r.ToLower() == "n"))
        {
            Console.WriteLine("Este programa esta diseñado para ser ejecutado junto con MariaDB en el puerto 3306\n¿Deseas Instalar MariaDB? n/s");
            r = Console.ReadLine();
        }
        if (r.ToLower() == "s")
        {
            Console.Clear();
            InstallAndStartMariaDB();
            VerificarExistenciaUsuario();
        }
    }
    else
    {
        VerificarExistenciaUsuario();
    }
}
else
{
    Console.WriteLine("Este programa esta diseñado para ser ejecutado en una distribucion Linux");
}