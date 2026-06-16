using LogicaServidor.Models.DTOs;
using LogicaServidor.Services;
using S7.Net;
using PlcDomain = LogicaServidor.Models.Domains.Plc;

namespace LogicaServidor.BackgroundServices;

public class InyeccionDatosPlcService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InyeccionDatosPlcService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        while (!stoppingToken.IsCancellationRequested)
        {
            InyeccionDatos();
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }

    public void InyeccionDatos()
    {
        using var scope = _scopeFactory.CreateScope();
        var _datosService = scope.ServiceProvider.GetRequiredService<DatosService>();
        var _relacionesService = scope.ServiceProvider.GetRequiredService<RelacionPlcAndSensorService>();
        var relaciones = _relacionesService.GetRelaciones();
        foreach (var relacion in relaciones)
        {
            EnviarDatosDTO datos = _datosService.GetDatosByNumeroSerie(relacion.Item2.NumeroSerie);
            PlcDomain x = relacion.Item1;
            if (datos.FechaMuestra.AddSeconds(10) > DateTime.Now)
            {
                var plc = new Plc(CpuType.S71200, x.Ip, 0, 1);
                plc.Open();
                plc.Write("DB10.DBW0", datos.Co2);
                plc.Write("DB10.DBW2", datos.Temperatura);
                plc.Write("DB10.DBW4", datos.Humedad);
                plc.Close();
            }
        }
    }
}