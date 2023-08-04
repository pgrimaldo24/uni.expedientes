using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Unir.Expedientes.Application.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal
{
    [Collection("CommonTestCollection")]
    public class EditAsignaturaExpedienteToSeguimientoByNotaFinalTest : TestBase
    {
        #region Handler
        [Fact(DisplayName = "Cuando se va a editar una Asignatura Expediente pero no existe, Retorna Unit Value")]
        public async Task HandleSinAsignaturaExpediente_OK()
        {
            //ARRANGE
            var sut = new Mock<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler>(Context)
            {
                CallBase = true
            };

            //ACT
            var result = await sut.Object.Handle(new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand(),
                CancellationToken.None);
            
            //ASSERT
            Assert.IsType<Unit>(result);
        }
        [Fact(DisplayName = "Cuando se edita Asignatura Expediente como no superada, Retorna Unit Value")]
        public async Task HandleNoSuperada_OK()
        {
            //ARRANGE
            var sut = new Mock<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler>(Context)
            {
                CallBase = true
            };

            var asignaturaExpediente = new AsignaturaExpediente
            {
                Id = 5
            };

            await Context.AsignaturasExpedientes.AddAsync(asignaturaExpediente);
            await Context.SaveChangesAsync();

            var request = new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand
            {
                IdAsignaturaExpedienteAlumno = 5,
                EsSuperada = false
            };

            //ACT
            var result = await sut.Object.Handle(request,
                CancellationToken.None);

            //ASSERT
            var edicion = Context.AsignaturasExpedientes.First(ae => ae.Id == asignaturaExpediente.Id);
            Assert.IsType<Unit>(result);
            Assert.True(edicion.SituacionAsignaturaId == SituacionAsignatura.NoSuperada);
        }
        [Fact(DisplayName = "Cuando se edita Asignatura Expediente como superada, Retorna Unit Value")]
        public async Task HandleSuperada_OK()
        {
            //ARRANGE
            var sut = new Mock<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler>(Context)
            {
                CallBase = true
            };

            var asignaturaExpediente = new AsignaturaExpediente
            {
                Id = 5
            };

            await Context.AsignaturasExpedientes.AddAsync(asignaturaExpediente);
            await Context.SaveChangesAsync();

            var request = new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand
            {
                IdAsignaturaExpedienteAlumno = 5,
                EsSuperada = true
            };

            //ACT
            var result = await sut.Object.Handle(request,
                CancellationToken.None);

            //ASSERT
            var edicion = Context.AsignaturasExpedientes.First(ae => ae.Id == asignaturaExpediente.Id);
            Assert.IsType<Unit>(result);
            Assert.True(edicion.SituacionAsignaturaId == SituacionAsignatura.Superada);
        }
        [Fact(DisplayName = "Cuando se edita Asignatura Expediente como no presentado, Retorna Unit Value")]
        public async Task HandleNoPresentado_OK()
        {
            //ARRANGE
            var sut = new Mock<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler>(Context)
            {
                CallBase = true
            };

            var asignaturaExpediente = new AsignaturaExpediente
            {
                Id = 5
            };

            await Context.AsignaturasExpedientes.AddAsync(asignaturaExpediente);
            await Context.SaveChangesAsync();

            var request = new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand
            {
                IdAsignaturaExpedienteAlumno = 5,
                EsNoPresentado = true
            };

            //ACT
            var result = await sut.Object.Handle(request,
                CancellationToken.None);

            //ASSERT
            var edicion = Context.AsignaturasExpedientes.First(ae => ae.Id == asignaturaExpediente.Id);
            Assert.IsType<Unit>(result);
            Assert.True(edicion.SituacionAsignaturaId == SituacionAsignatura.NoPresentada);
        }
        [Fact(DisplayName = "Cuando se edita Asignatura Expediente como Matrícula de Honor, Retorna Unit Value")]
        public async Task HandleMatriculaHonor_OK()
        {
            //ARRANGE
            var sut = new Mock<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler>(Context)
            {
                CallBase = true
            };

            var asignaturaExpediente = new AsignaturaExpediente
            {
                Id = 5
            };

            await Context.AsignaturasExpedientes.AddAsync(asignaturaExpediente);
            await Context.SaveChangesAsync();

            var request = new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand
            {
                IdAsignaturaExpedienteAlumno = 5,
                EsMatriculaHonor = true
            };

            //ACT
            var result = await sut.Object.Handle(request,
                CancellationToken.None);

            //ASSERT
            var edicion = Context.AsignaturasExpedientes.First(ae => ae.Id == asignaturaExpediente.Id);
            Assert.IsType<Unit>(result);
            Assert.True(edicion.SituacionAsignaturaId == SituacionAsignatura.MatriculaHonor);
        }
        #endregion
    }
}
