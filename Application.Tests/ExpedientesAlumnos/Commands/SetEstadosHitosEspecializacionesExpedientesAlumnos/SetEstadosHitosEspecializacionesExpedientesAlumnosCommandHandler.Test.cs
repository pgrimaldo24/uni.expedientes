using System;
using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadosHitosEspecializacionesExpedientesAlumnos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.SetEstadosHitosEspecializacionesExpedientesAlumnos
{
    [Collection("CommonTestCollection")]
    public class SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no se envían parámetros de búsqueda Retorna error")]
        public async Task Handle_SinParametros_Error()
        {
            //ARRANGE
            var request = new SetEstadosHitosEspecializacionesExpedientesAlumnosCommand();
            const string mensajeEsperado = "Debe especificar al menos un parámetro de búsqueda.";
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>>() { CallBase = true };
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService =
                new Mock<ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService>
                {
                    CallBase = true
                };
            var sut = new Mock<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>(mockLocalizer.Object,
                mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando la fecha de apertura final es menor que la inicial Retorna error")]
        public async Task Handle_FechaInicialMayorQueFinal_Error()
        {
            //ARRANGE
            var request = new SetEstadosHitosEspecializacionesExpedientesAlumnosCommand
            {
                FechaAperturaDesde = new DateTime(2020, 1, 2),
                FechaAperturaHasta = new DateTime(2020, 1, 1)
            };
            const string mensajeEsperado = "Debe especificar una fecha de apertura final mayor o igual a la inicial.";
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>>() { CallBase = true };
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService =
                new Mock<ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService>
                {
                    CallBase = true
                };
            var sut = new Mock<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>(mockLocalizer.Object,
                mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se setean los estados hitos y especializaciones de los expedientes encontrados Retorna ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new SetEstadosHitosEspecializacionesExpedientesAlumnosCommand
            {
                FechaAperturaDesde = new DateTime(2020, 1, 1),
                FechaAperturaHasta = new DateTime(2020, 1, 1),
                IdEstudio = 1,
                IdUniversidad = 4
            };
            var mockLocalizer = new Mock<IStringLocalizer<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>>() { CallBase = true };
            var mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService =
                new Mock<ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService>
                {
                    CallBase = true
                };
            var backgroundJobIdEsperado = Guid.NewGuid().ToString();
            mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService.Setup(s =>
                    s.SetEstadosHitosEspecializacionesDeExpedientesAlumnos(
                        It.IsAny<SetEstadosHitosEspecializacionesExpedientesAlumnosParameters>()))
                .Returns(backgroundJobIdEsperado);
            var sut = new Mock<SetEstadosHitosEspecializacionesExpedientesAlumnosCommandHandler>(mockLocalizer.Object,
                mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(backgroundJobIdEsperado, actual);
            mockISetEstadosHitosEspecializacionesDeExpedientesAlumnosApplicationService.Verify(s =>
                    s.SetEstadosHitosEspecializacionesDeExpedientesAlumnos(
                        It.IsAny<SetEstadosHitosEspecializacionesExpedientesAlumnosParameters>()), Times.Once);
        }

        #endregion
    }
}
