using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTipoVinculacionExpedienteAlumno;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditTipoVinculacionExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class EditTipoVinculacionByExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se intenta editar el tipo de vinculación de un expediente inexistente Devuelve error")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new EditTipoVinculacionByExpedienteAlumnoCommand(2, "123");

            var sut = new EditTipoVinculacionByExpedienteAlumnoCommandHandler(Context);

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message,
                exception.Message);
        }

        [Fact(DisplayName = "Cuando se edita el tipo de vinculación de un expediente Devuelve True")]
        public async Task Handle_True()
        {
            //ARRANGE
            var expedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            expedienteAlumno.SetupAllProperties();
            expedienteAlumno.Object.Id = 1;
            expedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == TipoSeguimientoExpediente.ExpedienteModificadoTipoVinculacion), 
                It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()));
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno.Object);
            await Context.SaveChangesAsync(CancellationToken.None);
            const string idRefTipoVinculacionEsperado = "123";
            var request = new EditTipoVinculacionByExpedienteAlumnoCommand(1, idRefTipoVinculacionEsperado);

            var sut = new EditTipoVinculacionByExpedienteAlumnoCommandHandler(Context);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            Assert.True(actual);
            Assert.Equal(idRefTipoVinculacionEsperado, expedientePersistido.IdRefTipoVinculacion);
            expedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == TipoSeguimientoExpediente.ExpedienteModificadoTipoVinculacion),
                    It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando el tipo de vinculación es el mismo que del expediente Devuelve False")]
        public async Task Handle_False()
        {
            //ARRANGE
            var expedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            expedienteAlumno.SetupAllProperties();
            expedienteAlumno.Object.Id = 1;
            expedienteAlumno.Object.IdRefTipoVinculacion = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno.Object);
            await Context.SaveChangesAsync(CancellationToken.None);
            var sut = new EditTipoVinculacionByExpedienteAlumnoCommandHandler(Context);
            var request = new EditTipoVinculacionByExpedienteAlumnoCommand(1, expedienteAlumno.Object.IdRefTipoVinculacion);

            //ACT
            var actual = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(actual);
            expedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == TipoSeguimientoExpediente.ExpedienteModificadoTipoVinculacion),
                    It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
