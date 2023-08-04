using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Commands.CreateTipoSituacionEstadoExpediente;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.TiposSituacionEstadoExpedientes.Commands.CreateTipoSituacionEstadoExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateTipoSituacionEstadoExpedienteCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe el tipo situación estado Devuelve una excepción")]
        public async Task Handle_NotFoundException_TipoSituacionEstado()
        {
            //ARRANGE
            var request = new CreateTipoSituacionEstadoExpedienteCommand();
            var sut = new CreateTipoSituacionEstadoExpedienteCommandHandler(Context);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no existe el expediente alumno Devuelve una excepción")]
        public async Task Handle_NotFoundException_ExpedienteAlumno()
        {
            //ARRANGE
            var request = new CreateTipoSituacionEstadoExpedienteCommand
            {
                TipoSituacionEstadoId = 1,
                ExpedienteAlumnoId = 1
            };
            await Context.TiposSituacionEstado.AddAsync(new TipoSituacionEstado
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync();
            var sut = new CreateTipoSituacionEstadoExpedienteCommandHandler(Context);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateTipoSituacionEstadoExpedienteCommand
            {
                TipoSituacionEstadoId = 1,
                ExpedienteAlumnoId = 1
            };
            await Context.TiposSituacionEstado.AddAsync(new TipoSituacionEstado
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync();
            var sutMock = new Mock<CreateTipoSituacionEstadoExpedienteCommandHandler>(Context) 
                { CallBase = true };
            sutMock.Setup(x => x.AssignTipoSituacionEstadoExpediente(
                It.IsAny<TipoSituacionEstado>(), It.IsAny<ExpedienteAlumno>(),
                It.IsAny<CreateTipoSituacionEstadoExpedienteCommand>())).Returns(new TipoSituacionEstadoExpediente());

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(x => x.AssignTipoSituacionEstadoExpediente(
                It.IsAny<TipoSituacionEstado>(), It.IsAny<ExpedienteAlumno>(),
                It.IsAny<CreateTipoSituacionEstadoExpedienteCommand>()), Times.Once);
        }

        #endregion

        #region AssignTipoSituacionEstadoExpediente

        [Fact(DisplayName = "Cuando se setean los datos Devuelve el objeto")]
        public void AssignTipoSituacionEstadoExpediente_Ok()
        {
            //ARRANGE
            var request = new CreateTipoSituacionEstadoExpedienteCommand
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddMonths(1),
                Descripcion = Guid.NewGuid().ToString()
            };
            var tipoSituacionEstado = new TipoSituacionEstado
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString()
            };
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            var sut = new CreateTipoSituacionEstadoExpedienteCommandHandler(Context);

            //ACT
            var actual = sut.AssignTipoSituacionEstadoExpediente(
                tipoSituacionEstado, expedienteAlumno, request);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(request.FechaInicio, actual.FechaInicio);
            Assert.Equal(request.FechaFin, actual.FechaFin);
            Assert.Equal(request.Descripcion, actual.Descripcion);
            Assert.Equal(tipoSituacionEstado.Id, actual.TipoSituacionEstado.Id);
            Assert.Equal(expedienteAlumno.Id, actual.ExpedienteAlumno.Id);
        }

        #endregion
    }
}
