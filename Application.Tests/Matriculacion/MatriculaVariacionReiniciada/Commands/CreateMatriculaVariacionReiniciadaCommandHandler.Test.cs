using MediatR;
using Moq;
using System.Threading.Tasks;
using System.Threading;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionReiniciada.Commands;
using System;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaVariacionReiniciada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaVariacionReiniciadaCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionReiniciadaCommand
            {
                AlumnoIdIntegracion = Guid.NewGuid().ToString(),
                MatriculaIdIntegracion = Guid.NewGuid().ToString(),
                FechaHora = DateTime.Now,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaVariacionReiniciadaCommandHandler>(Context, mockIMediator.Object) { CallBase = true };

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
