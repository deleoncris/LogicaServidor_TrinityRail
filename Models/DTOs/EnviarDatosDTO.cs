namespace LogicaServidor.Models.DTOs;

public class EnviarDatosDTO
{
    public int Co2 { get; set; }  = 0;
    public int Humedad { get; set; } = 0;
    public int Temperatura { get; set; } = 0;
    public DateTime FechaMuestra { get; set; }
}