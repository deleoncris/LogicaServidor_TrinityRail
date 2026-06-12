using LogicaServidor.Mappers;
using LogicaServidor.Models.DTOs;
using LogicaServidor.Models.Entities;
using LogicaServidor.Repositories;

namespace LogicaServidor.Services;

public class DatosService
{
    private readonly Repository<Datos> _repository;

    public DatosService(Repository<Datos>  repository)
    {
        _repository = repository;
    }
    public EnviarDatosDTO? GetDatosByNumeroSerie(string numeroSerie)
    {
        return _repository.GetAll()
            .Where(x => x.NumeroSerie == numeroSerie).OrderByDescending(x => x.Fecha).Select(x => new EnviarDatosDTO()
            {
                Humedad = x.Humedad,
                Co2 = x.Co2,
                Temperatura = x.Temperatura,
                FechaMuestra = x.Fecha
            }).FirstOrDefault();
    }
    public void GuardarDatos(DatosDTO datosDto, string numeroSerie)
    {
        Datos datos = DatosMapper.DTOToEntity(datosDto, numeroSerie);
        _repository.Insert(datos);
    }
}