using System.Collections.Generic;
using MediatR;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon;
using Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionAnulada.Commands;
using Unir.Expedientes.Application.Tests.Common;
using Xunit;
using System;

namespace Unir.Expedientes.Application.Tests.Matriculacion.MatriculaAmpliacionAnulada.Commands
{
    [Collection("CommonTestCollection")]
    public class CreateMatriculaAmpliacionAnuladaCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateMatriculaAmpliacionAnuladaCommand
            {
                AlumnoIdIntegracion = "1",
                MatriculaIdIntegracion = "1",
                IdsAsignaturasOfertadasAdicionadas = new List<int>(),
                Mensaje = Guid.NewGuid().ToString(),
                Origen = Guid.NewGuid().ToString()
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var sutMock = new Mock<CreateMatriculaAmpliacionAnuladaCommandHandler>(Context, mockIMediator.Object) { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<CreateMatriculaAnuladaCommonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<CreateMatriculaAnuladaCommonCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
