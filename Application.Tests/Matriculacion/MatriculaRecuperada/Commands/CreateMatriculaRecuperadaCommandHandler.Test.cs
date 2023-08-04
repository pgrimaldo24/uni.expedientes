using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;
using Unir.Expedientes.Application.Matriculacion.MatriculaRecuperada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaRecuperada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaRecuperadaCommandHandlerTests : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaRecuperadaCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadas = new List<int> { 1, 2 },
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaRecuperadaCommandHandler>(Context, mockIMediator.Object)
            { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<CreateMatriculaRecuperadaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<CreateMatriculaRecuperadaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
