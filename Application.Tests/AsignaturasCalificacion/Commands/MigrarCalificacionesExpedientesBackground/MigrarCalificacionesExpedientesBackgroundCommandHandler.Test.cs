using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionesExpedientesBackground;
using Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes;
using Unir.Expedientes.Application.Common.Queries.GetInfoSecurity;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Commands.MigrarCalificacionesExpedientesBackground
{
    [Collection("CommonTestCollection")]
    public class MigrarCalificacionesExpedientesBackgroundCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando los parámetros tienen valor Retorna String")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new MigrarCalificacionesExpedientesBackgroundCommand
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString()
            };

            var mockMigrarCalificacionService = new Mock<IMigrarCalificacionesExpedientesApplicationService>();
            mockMigrarCalificacionService.Setup(s => s.MigrarCalificacionesExpedientes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MigrarCalificacionesExpedientesParameters>()))
                .Returns(Guid.NewGuid().ToString());

            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionesExpedientesBackgroundCommandHandler>> { CallBase = true };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetInfoSecurityQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InfoSecurityDto());

            var sut = new MigrarCalificacionesExpedientesBackgroundCommandHandler(mockIStringLocalizer.Object, mockMigrarCalificacionService.Object, mockIMediator.Object);

            //ACT
            var actual = sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            mockMigrarCalificacionService.Verify(s => s.MigrarCalificacionesExpedientes(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MigrarCalificacionesExpedientesParameters>()), Times.Once);
            mockIMediator.Verify(s =>
                s.Send(It.IsAny<GetInfoSecurityQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando los parámetros no tienen valor Retorna BadRequest")]
        public async Task Handle_BadRequest()
        {
            //ARRANGE
            const string mensajeEsperado = "Debe especificar al menos un parámetro de búsqueda.";
            var request = new MigrarCalificacionesExpedientesBackgroundCommand();
            var mockMigrarCalificacionService = new Mock<IMigrarCalificacionesExpedientesApplicationService>();
            var mockIStringLocalizer = new Mock<IStringLocalizer<MigrarCalificacionesExpedientesBackgroundCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator> { CallBase = true };

            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new MigrarCalificacionesExpedientesBackgroundCommandHandler(mockIStringLocalizer.Object, mockMigrarCalificacionService.Object, mockIMediator.Object);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion
    }
}
