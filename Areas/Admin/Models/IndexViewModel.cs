using LogicaServidor.Models.Domains;

namespace LogicaServidor.Areas.Admin.Models;

public class IndexViewModel
{
    public List<Plc> Plcs { get; set; }
    public List<Dispositivo> Dispositivos { get; set; }
    public List<(Plc, Dispositivo)> Relaciones { get; set; }
}