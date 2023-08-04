using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditViaAccesoPlanExpedienteAlumno;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditViaAccesoPlanExpedienteAlumno
{
    [Collection("CommonTestCollection")]
    public class EditViaAccesoPlanExpedienteAlumnoCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se intenta editar la vía de acceso plan de un expediente inexistente Devuelve error")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new EditViaAccesoPlanExpedienteAlumnoCommand(2, "123");

            var sut = new EditViaAccesoPlanExpedienteAlumnoCommandHandler(Context);

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

        [Fact(DisplayName = "Cuando se edita la vía de acceso plan de un expediente Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new Mock<ExpedienteAlumno>
            {
                CallBase = true
            };
            expedienteAlumno.SetupAllProperties();
            expedienteAlumno.Object.Id = 1;
            expedienteAlumno.Setup(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == TipoSeguimientoExpediente.ExpedienteModificadoViaAcceso), 
                It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()));
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno.Object);
            await Context.SaveChangesAsync(CancellationToken.None);
            const string idRefViaAccesoPlanEsperada = "123";
            var request = new EditViaAccesoPlanExpedienteAlumnoCommand(1, idRefViaAccesoPlanEsperada);

            var sut = new EditViaAccesoPlanExpedienteAlumnoCommandHandler(Context);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientePersistido = await Context.ExpedientesAlumno.FirstAsync(CancellationToken.None);
            expedienteAlumno.Verify(s =>
                s.AddSeguimientoNoUser(It.Is<int>(i => i == TipoSeguimientoExpediente.ExpedienteModificadoViaAcceso), 
                It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(idRefViaAccesoPlanEsperada, expedientePersistido.IdRefViaAccesoPlan);
        }

        #endregion
    }
}
