namespace LogicaServidor.Services;

public class ServicioLoop:BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            GuardarDatosEnBd();
            MandarDatosAlPLC();
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void MandarDatosAlPLC()
    {
    }

    private void GuardarDatosEnBd()
    {
    }
}