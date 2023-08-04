using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionReiniciada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using System;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaAmpliacionReiniciada.Commands
{
    public class CreateMatriculaAmpliacionReiniciadaCommandHandlerTest : TestBase
    {
        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAmpliacionReiniciadaCommand
            {
                AlumnoIdIntegracion = Guid.NewGuid().ToString(),
                FechaHora = DateTime.Now,
                MatriculaIdIntegracion  = Guid.NewGuid().ToString(),
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaAmpliacionReiniciadaCommandHandler>(Context, mockIMediator.Object) { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<CreateMatriculaReiniciadaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<CreateMatriculaReiniciadaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
