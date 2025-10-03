using System;
using FluentValidation;
using MyDelivery.Domain.Enums;

namespace MyDelivery.Application.DTOs.Ocorrencia
{
    public class RegistrarOcorrenciaDto
    {
        public ETipoOcorrencia TipoOcorrencia { get; set; }
        public DateTime HoraOcorrencia { get; set; }
    }

    public class RegistrarOcorrenciaDtoValidator : AbstractValidator<RegistrarOcorrenciaDto>
    {
        public RegistrarOcorrenciaDtoValidator()
        {
            RuleFor(x => x.TipoOcorrencia)
                .IsInEnum()
                .WithMessage("Tipo de ocorrência inválido.");

            RuleFor(x => x.HoraOcorrencia)
                .NotEmpty().WithMessage("Hora da ocorrência é obrigatória.")
                .Must(d => d > DateTime.MinValue).WithMessage("Hora da ocorrência inválida.");
        }
    }
}
