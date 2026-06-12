using LogicaServidor.Helpers;
using LogicaServidor.Models.Domains;

namespace LogicaServidor.Services;

public class DispositivosService
{
    private List<Dispositivo> ListaDispositivos { get; set; } = [];

    public string AgregarDispositivo(string numeroSerieDispositivo)
    {
        var d = ListaDispositivos.FirstOrDefault(x=> x.NumeroSerie == numeroSerieDispositivo);
        if (d == null)
        {
            Dispositivo dispositivo = new(numeroSerieDispositivo);
            string token = "";
            do
            {
                token = TokenGenerator.GenerarStringAleatorio32();
            }
            while(ListaDispositivos.Any(x=>x.Token == token));

            dispositivo.Token = token;
            ListaDispositivos.Add(dispositivo);
            return token;
        }
        return "";
    }
    public bool VerificarExistenciaDispositivoByToken(string token)
    {
        return ListaDispositivos.Any(x=> x.Token == token);
    }
    public bool VerificarExistenciaDispositivoByNumeroSerie(string numeroSerie)
    {
        return ListaDispositivos.Any(x=> x.NumeroSerie == numeroSerie);
    }
    public bool Renovar(string token)
    {
        Dispositivo? d = ListaDispositivos.FirstOrDefault(x=>x.Token == token);
        if (d != null)
        {
            d.FechaRegistro = DateTime.Now;
            return true;
        }
        return false;
    }
    public void LimpiezaDispositivos()
    {
        var dispositivosEliminar = ListaDispositivos
            .Where(d => d.FechaRegistro.AddMinutes(3) < DateTime.Now)
            .ToList();

        foreach (var dispositivo in dispositivosEliminar)
        {
            ListaDispositivos.Remove(dispositivo);
        }
    }

    public string GetNumeroSerieByToken(string token)
    {
        return ListaDispositivos.FirstOrDefault(x => x.Token == token)!.NumeroSerie;
    }
}