using FluentValidation;
using LogicaServidor.Mappers;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Services;
using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Controllers;

[ApiController]
[Route("[controller]")]
public class SensoresController : ControllerBase
{
    private readonly DispositivosService _dispositivosService;
    private readonly DatosService _datosService;
    private readonly IValidator<DatosDTO> _datosValidator;

    public SensoresController(DispositivosService dispositivosService, DatosService datosService, IValidator<DatosDTO> datosValidator)
    {
        _dispositivosService = dispositivosService;
        _datosService = datosService;
        _datosValidator = datosValidator;
    }
    [HttpPost("GuardarDatos")]
    public IActionResult GuardarDatos(GuardarDatosDTO guardarDatosDto)
    {
        if (_dispositivosService.VerificarExistenciaDispositivoByToken(guardarDatosDto.Token))
        {
            DatosDTO datosDto = DatosMapper.GuardarDatosDTOToDatosDTO(guardarDatosDto);
            var validationResult = _datosValidator.Validate(datosDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            _datosService.GuardarDatos(datosDto, _dispositivosService.GetNumeroSerieByToken(guardarDatosDto.Token));
            return Ok("Guardado");
        }
        return BadRequest("Token inexistente");
    }
    [HttpGet("VerificarExistenciaSensor/{numeroSerie}")]
    public IActionResult VerificarExistenciaSensor(string numeroSerie)
    {
        if (_dispositivosService.VerificarExistenciaDispositivoByNumeroSerie(numeroSerie))
        {
            return Ok("Dispositivo funcionando");
        }
        return NotFound("Dispositivo no encontrado");
    }
    [HttpGet("DatosSensor/{numeroSerie}")]
    public IActionResult DatosSensor(string numeroSerie)
    {
        if (_dispositivosService.VerificarExistenciaDispositivoByNumeroSerie(numeroSerie))
        {
            var x =_datosService.GetDatosByNumeroSerie(numeroSerie);
            return Ok(x);
        }
        return NotFound("Dispositivo no encontrado");
    }
}