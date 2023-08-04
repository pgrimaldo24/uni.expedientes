using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class RelacionarExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el servicio no retorna los ids de planes No se relacionan los expedientes")]
        public async Task Handle_NoPlanes()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<RelacionarExpedienteAlumnoCommandHandler>(Context, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetIdsPlanesARelacionar(It.IsAny<int>(),
                It.IsAny<int>())).ReturnsAsync(new List<int>());

            var expediente = new ExpedienteAlumno 
            { 
                IdRefPlan = "1",
                IdRefEstudio = "1"
            };
            var request = new RelacionarExpedienteAlumnoCommand(expediente, new TipoRelacionExpediente());

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(x => x.GetIdsPlanesARelacionar(It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);
            sut.Verify(x => x.AssignExpedienteAlumnoRelacionado(It.IsAny<List<int>>(),
                It.IsAny<RelacionarExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Cuando existen los planes y realiza el proceso de relación entre expedientes Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<RelacionarExpedienteAlumnoCommandHandler>(Context, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetIdsPlanesARelacionar(It.IsAny<int>(),
                It.IsAny<int>())).ReturnsAsync(new List<int> {1, 2, 3});
            sut.Setup(x => x.AssignExpedienteAlumnoRelacionado(It.IsAny<List<int>>(),
                It.IsAny<RelacionarExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var expediente = new ExpedienteAlumno
            {
                IdRefPlan = "1",
                IdRefEstudio = "1"
            };
            var request = new RelacionarExpedienteAlumnoCommand(expediente, new TipoRelacionExpediente());

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(x => x.GetIdsPlanesARelacionar(It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);
            sut.Verify(x => x.AssignExpedienteAlumnoRelacionado(It.IsAny<List<int>>(),
                It.IsAny<RelacionarExpedienteAlumnoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region AssignExpedienteAlumnoRelacionado

        [Fact(DisplayName = "Cuando no existen expedientes filtrados por el id integracion alumno y la lista de planes Termina el proceso")]
        public async Task AssignExpedienteAlumnoRelacionado_Empty()
        {
            //ARRANGE
            var expedientes = new List<ExpedienteAlumno>
            {
                new()
                {
                    Id = 1,
                    IdRefPlan = "10"
                },
                new()
                {
                    Id = 2,
                    IdRefPlan = "11"
                }
            };
            await Context.ExpedientesAlumno.AddRangeAsync(expedientes);
            await Context.SaveChangesAsync(CancellationToken.None);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new RelacionarExpedienteAlumnoCommandHandler(Context, mockIErpAcademicoServiceClient.Object);
            var idsPlanes = new List<int> { 1, 2, 3 };
            var request = new RelacionarExpedienteAlumnoCommand(new ExpedienteAlumno(), new TipoRelacionExpediente());

            //ACT
            await sut.AssignExpedienteAlumnoRelacionado(idsPlanes, request, CancellationToken.None);

            //ASSERT
            Assert.True(expedientes.All(e => !idsPlanes.Contains(Convert.ToInt32(e.IdRefPlan))));
        }

        [Fact(DisplayName = "Cuando existen expedientes a relacionar Retonar Ok")]
        public async Task AssignExpedienteAlumnoRelacionado_Ok()
        {
            //ARRANGE
            var expedientes = new List<ExpedienteAlumno>
            {
                new()
                {
                    Id = 1,
                    IdRefPlan = "2",
                    IdRefIntegracionAlumno = "1"
                }
            };
            await Context.ExpedientesAlumno.AddRangeAsync(expedientes);
            var tiposRelaciones = new List<TipoRelacionExpediente>
            {
                new()
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                },
                new()
                {
                    Id = 2,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            await Context.TiposRelacionesExpediente.AddRangeAsync(tiposRelaciones);
            await Context.SaveChangesAsync(CancellationToken.None);
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new RelacionarExpedienteAlumnoCommandHandler(Context, mockIErpAcademicoServiceClient.Object);
            var idsPlanes = new List<int> { 1, 2, 3 };
            var mockExpediente = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            mockExpediente.Object.Id = 2;
            mockExpediente.Object.IdRefIntegracionAlumno = "1";
            mockExpediente.Object.IdRefPlan = "2";
            mockExpediente.SetupAllProperties();
            mockExpediente.Setup(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), null,
                It.IsAny<string>(), It.IsAny<string>()));
            var request = new RelacionarExpedienteAlumnoCommand(mockExpediente.Object, tiposRelaciones.Last());

            //ACT
            await sut.AssignExpedienteAlumnoRelacionado(idsPlanes, request, CancellationToken.None);
            await Context.SaveChangesAsync(CancellationToken.None);

            //ASSERT
            Assert.True(await Context.RelacionesExpedientes.AnyAsync());
            mockExpediente.Verify(x => x.AddSeguimientoNoUser(It.IsAny<int>(), It.IsAny<string>(), 
                null, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
