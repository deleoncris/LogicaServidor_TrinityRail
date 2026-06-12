using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController: ControllerBase
{
    [HttpGet("ComprobarConexionApi")]
    public IActionResult ComprobarConexionApi()
    {
        return Ok("Funcionando");
    }
}