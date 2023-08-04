using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.TareasDetalle.Commands.CreateTareaDetalle;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Tests.TareasDetalle.Commands.CreateTareaDetalle
{
    [Collection("CommonTestCollection")]
    public class CreateTareaDetalleCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando se crea la tarea detalle correctamente Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateTareaDetalleCommand
            {
                IdTarea = 1,
                IdExpediente = 1
            };

            var tarea = new Tarea
            {
                Id = 1,
                JobId = 1,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                Total = 1,
                Completadas = 1,
                Fallidas = 1,
                IdRefEstudio = 1,
                IdRefUniversidad = 1
            };
            await Context.Tareas.AddAsync(tarea);

            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expediente);
            await Context.SaveChangesAsync(CancellationToken.None);

            var tareaDetalle = new TareaDetalle
            {
                Id = 1,
                ExpedienteId = 1,
                TareaId = 1,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow,
                CompletadaOk = true,
                Mensaje = null
            };

            var sut = new Mock<CreateTareaDetalleCommandHandler>(Context) { CallBase = true };
            sut.Setup(s => s.AssignTareaDetalle(It.IsAny<CreateTareaDetalleCommand>(), It.IsAny<Tarea>(), It.IsAny<ExpedienteAlumno>()))
                .Returns(tareaDetalle);

            //ACT
            var act = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(act);
            Assert.Single(Context.TareasDetalle);
            sut.Verify(s => s.AssignTareaDetalle(It.IsAny<CreateTareaDetalleCommand>(), It.IsAny<Tarea>(),
                It.IsAny<ExpedienteAlumno>()), Times.Once);
        }
        [Fact(DisplayName = "Cuando la tarea no es encontrado Retorna NotFoundException")]
        public async Task Handle_Tarea_NotFound()
        {
            //ARRANGE
            var request = new CreateTareaDetalleCommand
            {
                IdTarea = 2
            };

            var tarea = new Tarea
            {
                Id = 1,
                JobId = 1,
                FechaInicio = DateTime.Now,
                Total = 1,
                Completadas = 1,
                Fallidas = 1
            };
            await Context.Tareas.AddAsync(tarea);
            await Context.SaveChangesAsync(CancellationToken.None);

            var sut = new CreateTareaDetalleCommandHandler(Context);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el expediente no es encontrado Retorna NotFoundException")]
        public async Task Handle_Expediente_NoFound()
        {
            //ARRANGE
            var request = new CreateTareaDetalleCommand
            {
                IdTarea = 1,
                IdExpediente = 2
            };

            var tarea = new Tarea
            {
                Id = 1,
                JobId = 1,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                Total = 1,
                Completadas = 1,
                Fallidas = 1,
                IdRefEstudio = 1,
                IdRefUniversidad = 1
            };
            await Context.Tareas.AddAsync(tarea);

            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expediente);
            await Context.SaveChangesAsync(CancellationToken.None);

            var sut = new CreateTareaDetalleCommandHandler(Context);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        #endregion

        #region AssignTareaDetalle
        [Fact(DisplayName = "Cuando se asigna valores a una tarea detalle Retorna Tarea Detalle")]
        public async Task AssignTareaDetalle()
        {
            //ARRANGE
            var request = new CreateTareaDetalleCommand
            {
                FechaInicio = DateTime.UtcNow.AddDays(1),
                FechaFin = DateTime.UtcNow,
                CompletadaOk = false,
                Mensaje = Guid.NewGuid().ToString()
            };

            var tarea = new Tarea
            {
                Id = 1,
                JobId = 1,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now,
                Total = 1,
                Completadas = 1,
                Fallidas = 1,
                IdRefEstudio = 1,
                IdRefUniversidad = 1
            };

            var expediente = new ExpedienteAlumno
            {
                Id = 1
            };

            var sut = new CreateTareaDetalleCommandHandler(Context);

            //ACT
            var act = sut.AssignTareaDetalle(request, tarea, expediente);

            //ASSERT
            Assert.NotNull(act);
            Assert.IsType<TareaDetalle>(act);
            Assert.Equal(request.FechaInicio, act.FechaInicio);
            Assert.Equal(request.FechaFin, act.FechaFin);
            Assert.Equal(request.CompletadaOk, act.CompletadaOk);
            Assert.Equal(request.Mensaje, act.Mensaje);
        }

        #endregion

    }
}
