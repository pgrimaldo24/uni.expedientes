using MediatR;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit
{
    [Collection("CommonTestCollection")]
    public class AddConsolidacionesRequisitosExpedienteUncommitCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existen requisitos a consolidar Termina el proceso")]
        public async Task Handle_Empty_Requisitos()
        {
            //ARRANGE
            var expediente = new ExpedienteAlumno();
            var request = new AddConsolidacionesRequisitosExpedienteUncommitCommand(expediente);
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetRequisitosComportamientosExpedientesAConsolidarQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RequisitoComportamientoExpediente>());
            var sut = new AddConsolidacionesRequisitosExpedienteUncommitCommandHandler(mockIMediator.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetRequisitosComportamientosExpedientesAConsolidarQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se consolidan los requisitos del expediente Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s =>
                    s.Send(It.IsAny<GetRequisitosComportamientosExpedientesAConsolidarQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1
                    },
                    new()
                    {
                        Id = 2
                    }
                });
            var sut = new AddConsolidacionesRequisitosExpedienteUncommitCommandHandler(mockIMediator.Object);

            var mockExpediente = new Mock<ExpedienteAlumno> { CallBase = true };
            mockExpediente.SetupAllProperties();
            mockExpediente.Object.Id = 1;
            mockExpediente.Object.ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>();
            mockExpediente.Setup(x =>
                x.AddConsolidacionRequisitoExpediente(It.IsAny<RequisitoComportamientoExpediente>()));
            var request = new AddConsolidacionesRequisitosExpedienteUncommitCommand(mockExpediente.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            mockExpediente.Verify(x =>
                x.AddConsolidacionRequisitoExpediente(It.IsAny<RequisitoComportamientoExpediente>()), Times.Exactly(2));
            mockIMediator.Verify(s => s.Send(
                It.IsAny<GetRequisitosComportamientosExpedientesAConsolidarQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
