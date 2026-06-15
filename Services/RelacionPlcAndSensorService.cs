using LogicaServidor.Models.Domains;

namespace LogicaServidor.Services;

public class RelacionPlcAndSensorService
{
    private List<(Plc, Dispositivo)> Relaciones { get; set; } = [];

    public void GuardarRelacion(List<(Plc, Dispositivo)> relaciones)
    {
        Relaciones = relaciones;
    }

    public List<(Plc, Dispositivo)> GetRelaciones()
    {
        return Relaciones;
    }
}