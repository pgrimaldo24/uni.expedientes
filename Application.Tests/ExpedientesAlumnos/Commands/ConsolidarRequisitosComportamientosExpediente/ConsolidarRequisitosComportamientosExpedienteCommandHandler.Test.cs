using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ConsolidarRequisitosComportamientosExpediente;
using Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.ConsolidarRequisitosComportamientosExpediente
{
    [Collection("CommonTestCollection")]
    public class ConsolidarRequisitosComportamientosExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe el expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new ConsolidarRequisitosComportamientosExpedienteCommand(1);
            var sut = new ConsolidarRequisitosComportamientosExpedienteCommandHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando se consolidan los requisitos del expediente Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new ConsolidarRequisitosComportamientosExpedienteCommand(1);
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s =>
                s.Send(It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);
            var sut = new ConsolidarRequisitosComportamientosExpedienteCommandHandler(Context, mockIMediator.Object);

            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>()
            });
            await Context.SaveChangesAsync();

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty((await Context.ExpedientesAlumno.FirstAsync()).ConsolidacionesRequisitosExpedientes);
            mockIMediator.Verify(s => s.Send(
                It.IsAny<AddConsolidacionesRequisitosExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
