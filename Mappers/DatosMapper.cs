using LogicaServidor.Models.DTOs;
using LogicaServidor.Models.Entities;

namespace LogicaServidor.Mappers;

public static class DatosMapper
{
    public static Datos DTOToEntity(DatosDTO dto, string serial)
    {
        return new Datos
        {
            NumeroSerie = serial,
            Humedad =  dto.Humedad,
            Temperatura = dto.Temperatura,
            Co2 = dto.Co2,
            Fecha =  DateTime.Now
        };
    }
    public static DatosDTO GuardarDatosDTOToDatosDTO(GuardarDatosDTO guardarDatosDto)
    {
        return new DatosDTO
        {
            Co2 = guardarDatosDto.Co2,
            Humedad = guardarDatosDto.Humedad,
            Temperatura = guardarDatosDto.Temperatura
        };
    }
}