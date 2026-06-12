using FluentValidation;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Controllers;

[ApiController]
[Route("[controller]")]
public class AutentificacionController : ControllerBase
{
    private readonly DispositivosService _dispositivosService;

    public AutentificacionController(DispositivosService dispositivosService)
    {
        _dispositivosService = dispositivosService;
    }

    [HttpPost("RegistrarDispositivo")]
    public IActionResult RegistrarDispositivo(string numeroSerieDispositivo)
    {
        if (string.IsNullOrWhiteSpace(numeroSerieDispositivo))
        {
            return BadRequest("El numero de serie no puede estar vacio");
        }

        string token = _dispositivosService.AgregarDispositivo(numeroSerieDispositivo);
        if (!string.IsNullOrWhiteSpace(token))
        {
            return Ok(token);
        }
        return BadRequest("No se pude registrar el dispositivo porque la IP o el Numero de Serie ya esta registrado");
    }

    [HttpPost("RenovarAutentificacion/{token}")]
    public IActionResult RenovarAutentificacion(string token)
    {
        bool seRenovo = _dispositivosService.Renovar(token);
        if (seRenovo)
        {
            return Ok("Renovado");
        }
        return NotFound("El dispositivo no existe o su autentificacion ya expiro");
    }
}