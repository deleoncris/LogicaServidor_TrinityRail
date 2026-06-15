using LogicaServidor.Areas.Admin.Models;
using LogicaServidor.Models.Domains;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogicaServidor.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RelationController : Controller
{
    private readonly IServiceScopeFactory _scopeFactory;

    public RelationController(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public IActionResult Index()
    {
        using var scope = _scopeFactory.CreateScope();
        var nmapService = scope.ServiceProvider.GetRequiredService<NmapService>();
        var dispositivosService = scope.ServiceProvider.GetRequiredService<DispositivosService>();
        var relacionPlcAndSensorService = scope.ServiceProvider.GetRequiredService<RelacionPlcAndSensorService>();
        List<Dispositivo> dispositivos = dispositivosService.GetListaDispositivos();
        List<Plc> plcs = nmapService.GetListaPlcs();
        List<(Plc, Dispositivo)> relations = relacionPlcAndSensorService.GetRelaciones();
        IndexViewModel vm = new()
        {
            Plcs = plcs,
            Dispositivos = dispositivos,
            Relaciones = relations
        };
        return View(vm);
    }
    [HttpGet]
    public IActionResult ObtenerEstado()
    {
        using var scope = _scopeFactory.CreateScope();

        var nmapService = scope.ServiceProvider.GetRequiredService<NmapService>();
        var dispositivosService = scope.ServiceProvider.GetRequiredService<DispositivosService>();
        var relacionPlcAndSensorService = scope.ServiceProvider.GetRequiredService<RelacionPlcAndSensorService>();

        return Json(new
        {
            plcs = nmapService.GetListaPlcs(),
            dispositivos = dispositivosService.GetListaDispositivos(),
            relaciones = relacionPlcAndSensorService.GetRelaciones()
        });
    }
    [HttpPost]
    public IActionResult GuardarRelacion(List<(Plc, Dispositivo)> relations)
    {
        using var scope = _scopeFactory.CreateScope();
        var relacionPlcAndSensorService = scope.ServiceProvider.GetRequiredService<RelacionPlcAndSensorService>();
        relacionPlcAndSensorService.GuardarRelacion(relations);
        return Ok();
    }
}