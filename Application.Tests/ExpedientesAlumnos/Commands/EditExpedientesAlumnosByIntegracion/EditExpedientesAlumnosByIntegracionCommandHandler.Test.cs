using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion
{
    [Collection("CommonTestCollection")]
    public class EditExpedientesAlumnosByIntegracionCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no se envía ids de expedientes No realiza ninguna acción")]
        public async Task Handle_SinExpedientes()
        {
            //ARRANGE
            var request =
                new EditExpedientesAlumnosByIntegracionCommand(
                    new List<EditExpedienteAlumnoByIdIntegracionParametersDto>());
            var sut = new EditExpedientesAlumnosByIntegracionCommandHandler(Context);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(request.ExpedientesAlumnos);
        }

        [Fact(DisplayName = "Cuando se modifican los expedientes por integración Realiza el proceso ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var idsExpedientes = new[] { 1, 2, 3 };
            var expedientesRequest = idsExpedientes.Select(i => new EditExpedienteAlumnoByIdIntegracionParametersDto
            {
                Id = i,
                IdRefVersionPlan = $"50{i}",
                NroVersion = i == 1 ? i : null
            }).ToList();
            var expedientesPersistidos = new List<ExpedienteAlumno>();
            Enumerable.Range(1, idsExpedientes.Length - 1).ToList().ForEach(i =>
            {
                var mockExpediente = new Mock<ExpedienteAlumno>
                {
                    CallBase = true
                };
                mockExpediente.SetupAllProperties();
                mockExpediente.Object.Id = i;
                mockExpediente.Object.IdRefVersionPlan = $"60{i}";
                mockExpediente.Setup(s =>
                    s.AddSeguimientoNoUser(It.Is<int>(ts => ts == TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan),
                        It.IsAny<string>(), null, It.IsAny<string>(), It.IsAny<string>()));
                expedientesPersistidos.Add(mockExpediente.Object);
            });
            await Context.ExpedientesAlumno.AddRangeAsync(expedientesPersistidos);
            await Context.SaveChangesAsync(CancellationToken.None);
            var request =
                new EditExpedientesAlumnosByIntegracionCommand(
                    expedientesRequest);
            var sut = new EditExpedientesAlumnosByIntegracionCommandHandler(Context);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            var expedientesModificados = await Context.ExpedientesAlumno.ToListAsync(CancellationToken.None);
            var idsRefVersionPlanModificados = expedientesModificados.Select(e => e.IdRefVersionPlan).ToArray();
            Assert.DoesNotContain("601", idsRefVersionPlanModificados);
            Assert.Contains("501", idsRefVersionPlanModificados);
        }

        #endregion
    }
}
