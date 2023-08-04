using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using MediatR;
using Moq;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRecuperada;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaVariacionRecuperada
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaVariacionRecuperadaCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando todo es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaVariacionRecuperadaCommand
            {
                MatriculaIdIntegracion = "1",
                AlumnoIdIntegracion = "1",
                IdsAsignaturasOfertadasAdicionadas = new List<int> { 1, 2 },
                IdsAsignaturasOfertadasExcluidas = new List<int> { 3, 4 },
                FechaHora = DateTime.UtcNow,
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaVariacionRecuperadaCommandHandler>(Context, mockIMediator.Object)
                { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<CreateMatriculaRecuperadaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<CreateMatriculaRecuperadaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
