using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.ValidarConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Commands.ValidarConsolidacionRequisitoExpediente
{
    [Collection("CommonTestCollection")]
    public class ValidarConsolidacionRequisitoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new ValidarConsolidacionRequisitoExpedienteCommand();
            var sut = new ValidarConsolidacionRequisitoExpedienteCommandHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new ValidarConsolidacionRequisitoExpedienteCommand
            {
                Id = 1
            };
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1,
                    RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>()
                },
                ConsolidacionesRequisitosExpedientesDocumentos = new List<ConsolidacionRequisitoExpedienteDocumento>()
            });
            await Context.SaveChangesAsync();

            var sutMock = new Mock<ValidarConsolidacionRequisitoExpedienteCommandHandler>(Context, null)
            { CallBase = true };
            sutMock.Setup(x => x.ValidatePropiedades(
                    It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<ValidarConsolidacionRequisitoExpedienteCommand>()))
                .Returns(Task.CompletedTask);
            sutMock.Setup(x => x.AssignConsolidacionRequisitoExpediente(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<int?>()));

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<ValidarConsolidacionRequisitoExpedienteCommand>()), Times.Once);
            sutMock.Verify(x => x.AssignConsolidacionRequisitoExpediente(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<int?>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando el estado no es pendiente o rechazado Devuelve excepción")]
        public async Task ValidatePropiedades_EstadoInvalido_BadRequestException()
        {
            //ARRANGE
            var request = new ValidarConsolidacionRequisitoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new ValidarConsolidacionRequisitoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "El estado debe estar en estado Pendiente o Rechazado";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockConsolidacion = new Mock<ConsolidacionRequisitoExpediente>
            {
                CallBase = true
            };
            mockConsolidacion.SetupAllProperties();
            mockConsolidacion.Setup(x => x.EsValidoParaRechazarValidar()).Returns(false);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedades(mockConsolidacion.Object, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockConsolidacion.Verify(x => x.EsValidoParaRechazarValidar(), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe la causa estado requisito Devuelve excepción")]
        public async Task ValidatePropiedades_CausaEstado_BadRequestException()
        {
            //ARRANGE
            var request = new ValidarConsolidacionRequisitoExpedienteCommand
            {
                IdCausaEstadoRequisito = 2
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new ValidarConsolidacionRequisitoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "No existe la causa del estado del requisito";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockConsolidacion = new Mock<ConsolidacionRequisitoExpediente>
            {
                CallBase = true
            };
            mockConsolidacion.SetupAllProperties();
            mockConsolidacion.Setup(x => x.EsValidoParaRechazarValidar()).Returns(true);
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(
                new CausaEstadoRequisitoConsolidadaExpediente
                {
                    Id = 1
                });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedades(mockConsolidacion.Object, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockConsolidacion.Verify(x => x.EsValidoParaRechazarValidar(), Times.Once);
        }

        [Fact(DisplayName = "Cuando da error en las validaciones de dominio Devuelve excepción")]
        public async Task ValidatePropiedades_Dominio_BadRequestException()
        {
            //ARRANGE
            var request = new ValidarConsolidacionRequisitoExpedienteCommand
            {
                IdCausaEstadoRequisito = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<ValidarConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new ValidarConsolidacionRequisitoExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            const string mensajeEsperado = "Errores de dominio";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockConsolidacion = new Mock<ConsolidacionRequisitoExpediente>
            {
                CallBase = true
            };
            mockConsolidacion.SetupAllProperties();
            mockConsolidacion.Setup(x => x.EsValidoParaRechazarValidar()).Returns(true);
            mockConsolidacion.Setup(x => x.VerificarPropiedadesParaEstadoValidado()).Returns(new List<string>
            {
                mensajeEsperado
            });
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(
                new CausaEstadoRequisitoConsolidadaExpediente
                {
                    Id = 1
                });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedades(mockConsolidacion.Object, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<ValidationErrorsException>(ex);
            mockConsolidacion.Verify(x => x.EsValidoParaRechazarValidar(), Times.Once);
            mockConsolidacion.Verify(x => x.VerificarPropiedadesParaEstadoValidado(), Times.Once);
        }

        #endregion
    }
}
