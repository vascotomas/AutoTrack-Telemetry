using FastEndpoints;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace AutoTelemetryAPI.Features.IngestTelemetry
{
    public class IngestTelemetryValidator : Validator<IngestTelemetryRequest>
    {
        public IngestTelemetryValidator() 
        {
            RuleFor(x => x.ChasisId)
            .NotEmpty().WithMessage("El ID del Chasis es obligatorio.")
            .MinimumLength(3).WithMessage("El ID del Chasis debe tener al menos 3 caracteres.");

            RuleFor(x => x.Estacion)
                .NotEmpty().WithMessage("La Estación de lectura es obligatoria.");

            RuleFor(x => x.Temperatura)
                .InclusiveBetween(-50, 200).WithMessage("La temperatura reportada está fuera del rango de los sensores (-50 a 200).");
        }
    }
}
