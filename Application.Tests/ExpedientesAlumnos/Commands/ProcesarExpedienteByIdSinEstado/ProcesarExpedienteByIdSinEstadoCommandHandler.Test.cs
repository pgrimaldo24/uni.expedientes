using MediatR;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarExpedienteByIdSinEstado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.ProcesarExpedienteByIdSinEstado
{
    [Collection("CommonTestCollection")]
    public class ProcesarExpedienteByIdSinEstadoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no encuentra expediente Retorna excepción")]
        public async Task Handle_SinExpedinete()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(m => m.Send(It.IsAny<GetExpedientesSinEstadosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpedienteAlumno>());
            mockIMediator.Setup(m => m.Send(It.IsAny<SetEstadoHitosEspecializacionesExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var sut = new ProcesarExpedienteByIdSinEstadoCommandHandler(mockIMediator.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(new ProcesarExpedienteByIdSinEstadoCommand
                {
                    IdExpedienteAlumno = 1
                }, CancellationToken.None);
            });

            //ASSERT
            mockIMediator.Verify(m => m.Send(It.IsAny<GetExpedientesSinEstadosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(m => m.Send(It.IsAny<SetEstadoHitosEspecializacionesExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Contains("not found", ex.Message);
        }

        [Fact(DisplayName = "Cuando encuentra expediente Retorna ok")]
        public async Task Handle_ConExpedinete()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(m => m.Send(It.IsAny<GetExpedientesSinEstadosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpedienteAlumno>
                {
                    new ExpedienteAlumno()
                });
            mockIMediator.Setup(m => m.Send(It.IsAny<SetEstadoHitosEspecializacionesExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var sut = new ProcesarExpedienteByIdSinEstadoCommandHandler(mockIMediator.Object);

            //ACT
            await sut.Handle(new ProcesarExpedienteByIdSinEstadoCommand
            {
                IdExpedienteAlumno = 1
            }, CancellationToken.None);

            //ASSERT
            mockIMediator.Verify(m => m.Send(It.IsAny<GetExpedientesSinEstadosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(m => m.Send(It.IsAny<SetEstadoHitosEspecializacionesExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()), Times.Once);

        }
    }

    #endregion

}
