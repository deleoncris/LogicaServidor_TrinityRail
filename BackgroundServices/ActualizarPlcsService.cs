using LogicaServidor.Services;

namespace LogicaServidor.BackgroundServices;

public class ActualizarPlcsService : BackgroundService
{
    private readonly NmapService _nmapService;

    public ActualizarPlcsService(NmapService nmapService)
    {
        _nmapService = nmapService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            _nmapService.EscaneoRed();
        }
    }
}