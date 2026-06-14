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
            Test();
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }

    public void Test()
    {
        /*Metodo unicamente de prueba*/
        using var scope = _scopeFactory.CreateScope();
        var _nmapService = scope.ServiceProvider.GetRequiredService<NmapService>();
        var _datosService = scope.ServiceProvider.GetRequiredService<DatosService>();
        EnviarDatosDTO datos = _datosService.GetDatosByNumeroSerie("001");
        List<PlcDomain> lista = _nmapService.EscaneoRed();
        PlcDomain x = lista.FirstOrDefault(x=>x.Hostname == "plc-trinity-rail-001");
        if (x != null)
        {
            if (datos.FechaMuestra.AddMinutes(1) > DateTime.Now)
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