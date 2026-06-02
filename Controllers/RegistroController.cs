using LogicaServidor.Models;
using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Controllers;
[ApiController]
[Route("[controller]")]
public class RegistroController : Controller
{
    private static readonly List<Dispositivo> ListaDispositivos = [];
    [HttpPost("Registrar")]
    public IActionResult Registrar(string serie, string ip)
    {
        Dispositivo d = new()
        {
            NumeroSerie = serie,
            IP = ip
        };

        ListaDispositivos.Add(d);

        Response.Headers.Connection = "close";

        return Content("OK", "text/plain");
    }

    [HttpGet("Registrar")]
    public IActionResult RegistrarGet(string serie, string ip)
    {
        return Registrar(serie, ip);
    }
    
    [HttpGet("Sensores")]
    public async Task<IActionResult> GetIpByNumeroSerie(string numeroSerie)
    {
        var dispositivo = ListaDispositivos.FirstOrDefault(d => d.NumeroSerie == numeroSerie);

        if (dispositivo == null)
        {
            return NotFound(new { error = "Dispositivo no encontrado" });
        }

        return Ok(dispositivo.IP);
    }

    [HttpGet("ValoresSensor")]
    public async Task<IActionResult> GetDatosByNumeroSerie(string numeroSerie)
    {
        Dispositivo? d = ListaDispositivos.FirstOrDefault(d => d.NumeroSerie == numeroSerie);

        if (d == null)
        {
            return Ok("Error");
        }

        Datos datos = new();

        try
        {
            using HttpClient httpClient = new();

            datos.CO2 = int.Parse(await httpClient.GetStringAsync($"http://{d.IP}:8080/co2"));
            datos.Temperatura = int.Parse(await httpClient.GetStringAsync($"http://{d.IP}:8080/temperatura"));
            datos.Humedad = int.Parse(await httpClient.GetStringAsync($"http://{d.IP}:8080/humedad"));
        }
        catch
        {
            return Ok("Error");
        }
        return Ok(datos);
    }
    
    [HttpGet("Server")]
    public IActionResult GetServer()
    {
        return Ok("Ok");
    }

    [HttpGet("SensorExistencia")]
    public IActionResult GetSensorExistencia(string numeroSerie)
    {
        if (ListaDispositivos.FirstOrDefault(x => x.NumeroSerie == numeroSerie) == null)
        {
            return NotFound(new { error = "Dispositivo no encontrado" });
        }
        return Ok("Ok");
    }
}