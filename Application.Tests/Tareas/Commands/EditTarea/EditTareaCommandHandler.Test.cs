using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Tareas.Commands.EditTarea;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Tareas.Commands.EditTarea
{
    [Collection("CommonTestCollection")]
    public class EditTareaCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando se actualiza la tarea correctamente Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new EditTareaCommand
            {
                IdTarea = 1,
                FechaFin = DateTime.UtcNow.AddDays(1),
                Completadas = 10,
                Fallidas = 1,
                Total = 11
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
            await Context.SaveChangesAsync(CancellationToken.None);

            var sut = new Mock<EditTareaCommandHandler>(Context) { CallBase = true };
            sut.Setup(s => s.AssignTarea(It.IsAny<EditTareaCommand>(), It.IsAny<Tarea>()));

            //ACT
            var act = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.IsType<Unit>(act);
            sut.Verify(s => s.AssignTarea(It.IsAny<EditTareaCommand>(), It.IsAny<Tarea>()), Times.Once);
        }
        #endregion

        #region AssignTarea
        [Fact(DisplayName = "Cuando se asigna valores para actualizar una tarea Retorna Ok")]
        public async Task AssignTarea_Ok()
        {
            //ARRANGE
            var request = new EditTareaCommand
            {
                FechaFin = DateTime.UtcNow.AddDays(1),
                Total = 10,
                Completadas = 8,
                Fallidas = 2
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
            var sut = new EditTareaCommandHandler(Context);

            //ACT
            sut.AssignTarea(request, tarea);

            //ASSERT
            Assert.Equal(request.FechaFin, tarea.FechaFin);
            Assert.Equal(request.Total, tarea.Total);
            Assert.Equal(request.Completadas, tarea.Completadas);
            Assert.Equal(request.Fallidas, tarea.Fallidas);
        }

        #endregion

    }
}
