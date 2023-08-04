using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Tareas.Commands.CreateTarea;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Tareas.Commands.CreateTarea
{
    [Collection("CommonTestCollection")]
    public class CreateTareaCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando se crea la tarea correctamente Retorna Tarea")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateTareaCommand();
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
            var sut = new Mock<CreateTareaCommandHandler>(Context) { CallBase = true };
            sut.Setup(s => s.AssignTarea(It.IsAny<CreateTareaCommand>()))
                .Returns(tarea);

            //ACT
            var act = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(act);
            Assert.IsType<Tarea>(act);
            Assert.Equal(tarea.Id, act.Id);
            Assert.Equal(tarea.FechaInicio, act.FechaInicio);
            Assert.Equal(tarea.FechaFin, act.FechaFin);
            Assert.Equal(tarea.Total, act.Total);
            Assert.Equal(tarea.Completadas, act.Completadas);
            Assert.Equal(tarea.Fallidas, act.Fallidas);
            sut.Verify(s => s.AssignTarea(It.IsAny<CreateTareaCommand>()),Times.Once);
        }

        #endregion

        #region AssignTarea
        [Fact(DisplayName = "Cuando se asigna valores a una tarea Retorna Tarea")]
        public async Task AssignTarea_Ok()
        {
            //ARRANGE
            var request = new CreateTareaCommand
            {
                JobId = 1,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow,
                Total = 1,
                Completadas = 1,
                Fallidas = 1,
                IdRefUniversidad = 1,
                IdRefEstudio = 1
            };

            var sut = new CreateTareaCommandHandler(Context);

            //ACT
            var act = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(act);
            Assert.IsType<Tarea>(act);
            Assert.Equal(1, act.Id);
            Assert.Equal(1, act.IdRefUniversidad);
            Assert.Equal(1, act.IdRefEstudio);
        }

        #endregion
    }
}
