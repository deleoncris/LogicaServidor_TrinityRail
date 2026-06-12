using System;
using System.Collections.Generic;

namespace LogicaServidor.Models.Entities;

public partial class Datos
{
    public int Id { get; set; }

    public string NumeroSerie { get; set; } = null!;

    public int Humedad { get; set; }

    public int Temperatura { get; set; }

    public int Co2 { get; set; }

    public DateTime Fecha { get; set; }
}
