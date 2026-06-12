namespace LogicaServidor.Models.DTOs;

public class GuardarDatosDTO
{
    public string Token { get; set; } = "";
    public int Co2 { get; set; }
    public int Humedad {get; set;}
    public int Temperatura { get; set; }
}