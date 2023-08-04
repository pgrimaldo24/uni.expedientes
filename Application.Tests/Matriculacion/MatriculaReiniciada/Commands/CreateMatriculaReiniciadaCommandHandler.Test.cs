using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Application.Matriculacion.MatriculaReiniciada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaReiniciada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaReiniciadaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaReiniciadaCommand
            {
                AlumnoIdIntegracion = Guid.NewGuid().ToString(),
                MatriculaIdIntegracion = Guid.NewGuid().ToString(),
                FechaHora = DateTime.Now,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaReiniciadaCommandHandler>(Context, mockIMediator.Object) { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<CreateMatriculaReiniciadaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<CreateMatriculaReiniciadaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
