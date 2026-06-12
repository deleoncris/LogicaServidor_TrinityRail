namespace LogicaServidor.Models.Domains;

public class Dispositivo
{
    public Dispositivo(string numeroSerie = "")
    {
        NumeroSerie = numeroSerie;
    }
    public string NumeroSerie { get; set; }
    public string Token { get; set; } = "";
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}