using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud;
using Unir.Expedientes.Application.Common.Queries.GetRolesSecurity;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidarGenerarSolicitudTituloCertificado;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.Crosscutting.Bus;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.GenerarSolicitudTituloCertificado
{
    [Collection("CommonTestCollection")]
    public class GenerarSolicitudTituloCertificadoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se publica la generación de solicitud Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefPlan = Guid.NewGuid().ToString(),
                IdRefIntegracionAlumno = Guid.NewGuid().ToString()
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync(CancellationToken.None);

            var tipoSolicitud = new TipoSolicitudDto
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                RefCodigoTipoSolicitud = "SCAP"
            };

            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIBusClientService = new Mock<IBusClientService> { CallBase = true };

            var sut = new GenerarSolicitudTituloCertificadoCommandHandler(
                Context, mockIMediator.Object, mockIBusClientService.Object);

            var roles = new[] { AppConfiguration.KeyAdminRole, AppConfiguration.KeyGestorRole };

            mockIMediator.Setup(s => s.Send(It.IsAny<ValidarGenerarSolicitudTituloCertificadoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetTipoSolicitudQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tipoSolicitud);

            mockIMediator.Setup(s => s.Send(It.IsAny<GetRolesSecurityQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var request = new GenerarSolicitudTituloCertificadoCommand
            {
                IdsExpedientes = new System.Collections.Generic.List<int> { 1, 2 },
                TipoSolicitud = 1,
                FechaSolicitud = DateTime.UtcNow,
                FechaPago = DateTime.UtcNow
            };

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(actual);
            mockIMediator.Verify(s => s.Send(It.IsAny<ValidarGenerarSolicitudTituloCertificadoCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetTipoSolicitudQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetRolesSecurityQuery>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
