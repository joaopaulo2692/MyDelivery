using System;
using FluentValidation.TestHelper;
using Xunit;
using MyDelivery.Application.DTOs;
using MyDelivery.Domain.Enums;

namespace MyDelivery.UnitTests.Validators
{
    public class RegistrarOcorrenciaDtoValidatorTests
    {
        private readonly RegistrarOcorrenciaDtoValidator _validator;

        public RegistrarOcorrenciaDtoValidatorTests()
        {
            _validator = new RegistrarOcorrenciaDtoValidator();
        }

        [Fact]
        public void Deve_Falhar_Se_HoraOcorrenciaVazia()
        {
            var dto = new RegistrarOcorrenciaDto
            {
                TipoOcorrencia = ETipoOcorrencia.EmRotaDeEntrega,
                HoraOcorrencia = default
            };

            var result = _validator.TestValidate(dto);
            result.ShouldHaveValidationErrorFor(x => x.HoraOcorrencia);
        }

        [Fact]
        public void Deve_Passar_Se_DtoValido()
        {
            var dto = new RegistrarOcorrenciaDto
            {
                TipoOcorrencia = ETipoOcorrencia.EntregueComSucesso,
                HoraOcorrencia = DateTime.Now
            };

            var result = _validator.TestValidate(dto);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
