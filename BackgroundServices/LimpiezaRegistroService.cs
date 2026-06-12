using LogicaServidor.Services;

namespace LogicaServidor.BackgroundServices;

public class LimpiezaRegistroService : BackgroundService
{
    private readonly DispositivosService _dispositivosService;

    public LimpiezaRegistroService(DispositivosService dispositivosService)
    {
        _dispositivosService = dispositivosService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            _dispositivosService.LimpiezaDispositivos();
        }
    }
}