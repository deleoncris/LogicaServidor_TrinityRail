using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LogicaServidor.BackgroundServices;

public class AnunciosRedLocalServices : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        string anuncio = "Servidor Trinity Rail";
        byte[] data = Encoding.UTF8.GetBytes(anuncio);
        while (!stoppingToken.IsCancellationRequested)
        {
            await udpClient.SendAsync(data, data.Length, new(IPAddress.Broadcast, 50000));
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}