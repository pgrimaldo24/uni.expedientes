using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedientesAlumnosBackground;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.RelacionarExpedientesAlumnosBackground
{
    [Collection("CommonTestCollection")]
    public class RelacionarExpedientesAlumnosBackgroundCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no se envían parámetros de búsqueda Retorna error")]
        public async Task Handle_SinParametros_Error()
        {
            //ARRANGE
            var request = new RelacionarExpedientesAlumnosBackgroundCommand();
            const string mensajeEsperado = "Debe especificar al menos un parámetro de búsqueda.";
            var mockLocalizer = new Mock<IStringLocalizer<RelacionarExpedientesAlumnosBackgroundCommandHandler>> { CallBase = true };
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIRelacionExpedientesAlumnosApplicationService =
                new Mock<IRelacionarExpedientesAlumnosApplicationService>
                {
                    CallBase = true
                };
            var sut = new Mock<RelacionarExpedientesAlumnosBackgroundCommandHandler>(mockLocalizer.Object,
                mockIRelacionExpedientesAlumnosApplicationService.Object)
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
            var request = new RelacionarExpedientesAlumnosBackgroundCommand
            {
                FechaAperturaDesde = new DateTime(2020, 1, 2),
                FechaAperturaHasta = new DateTime(2020, 1, 1)
            };
            const string mensajeEsperado = "Debe especificar una fecha de apertura final mayor o igual a la inicial.";
            var mockLocalizer = new Mock<IStringLocalizer<RelacionarExpedientesAlumnosBackgroundCommandHandler>> { CallBase = true };
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIRelacionExpedientesAlumnosApplicationService =
                new Mock<IRelacionarExpedientesAlumnosApplicationService>
                {
                    CallBase = true
                };
            var sut = new Mock<RelacionarExpedientesAlumnosBackgroundCommandHandler>(mockLocalizer.Object,
                mockIRelacionExpedientesAlumnosApplicationService.Object)
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

        [Fact(DisplayName = "Cuando se relacionan los expedientes encontrados Retorna ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new RelacionarExpedientesAlumnosBackgroundCommand
            {
                FechaAperturaDesde = new DateTime(2020, 1, 1),
                FechaAperturaHasta = new DateTime(2020, 1, 1),
                IdRefEstudio = "1",
                IdRefUniversidad = "1"
            };
            var mockLocalizer = new Mock<IStringLocalizer<RelacionarExpedientesAlumnosBackgroundCommandHandler>> { CallBase = true };
            var mockIRelacionExpedientesAlumnosApplicationService = new Mock<IRelacionarExpedientesAlumnosApplicationService> 
            { CallBase = true };

            var backgroundJobIdEsperado = Guid.NewGuid().ToString();
            mockIRelacionExpedientesAlumnosApplicationService.Setup(s =>
                    s.RelacionarExpedientesAlumnos(It.IsAny<RelacionarExpedientesAlumnosParameters>()))
                .Returns(backgroundJobIdEsperado);

            var sut = new Mock<RelacionarExpedientesAlumnosBackgroundCommandHandler>(mockLocalizer.Object,
                mockIRelacionExpedientesAlumnosApplicationService.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(backgroundJobIdEsperado, actual);
            mockIRelacionExpedientesAlumnosApplicationService.Verify(s =>
                s.RelacionarExpedientesAlumnos(It.IsAny<RelacionarExpedientesAlumnosParameters>()), Times.Once);
        }

        #endregion
    }
}
