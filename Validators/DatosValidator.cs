using FluentValidation;
using LogicaServidor.Models.DTOs;

namespace LogicaServidor.Validators;

public class DatosValidator: AbstractValidator<DatosDTO>
{
    public DatosValidator()
    {
        RuleFor(x => x.Humedad)
            .NotEmpty()
            .WithMessage("La humedad es obligatoria")
            .GreaterThanOrEqualTo(20)
            .WithMessage("La humedad no puede ser menor a 20% por limitaciones de hardware")
            .LessThanOrEqualTo(90)
            .WithMessage("La humedad no puede ser mayor a 90% por limitaciones de hardware");
        RuleFor(x => x.Co2)
            .NotEmpty()
            .WithMessage("El co2 es obligatorio")
            .GreaterThanOrEqualTo(0)
            .WithMessage("El co2 no puede ser menor a 0")
            .LessThanOrEqualTo(500)
            .WithMessage("El co2 no puede ser menor a 500");
        RuleFor(x=>x.Temperatura)
            .NotEmpty()
            .WithMessage("La temperatura es obligatoria")
            .GreaterThanOrEqualTo(0)
            .WithMessage("La temperatura no puede ser menor a 0 C° por limitaciones de hardware")
            .LessThanOrEqualTo(50)
            .WithMessage("La temperatura no puede ser menor a 50 C° por limitaciones de hardware");
            
    }
}