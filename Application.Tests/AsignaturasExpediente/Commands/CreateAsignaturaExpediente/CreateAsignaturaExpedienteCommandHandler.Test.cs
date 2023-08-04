using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasExpediente.Commands.CreateAsignaturaExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasExpediente.Commands.CreateAsignaturaExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateAsignaturaExpedienteCommandHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los datos enviados en el request están correctos Realiza el insert en bd")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new CreateAsignaturaExpedienteCommand
            {
                IdRefAsignaturaPlan = null,
                NombreAsignatura = "Matemática I",
                CodigoAsignatura = "UNIRM_01",
                OrdenAsignatura = 1,
                Ects = 1,
                IdRefTipoAsignatura = "refTipoAsig",
                SimboloTipoAsignatura = "simboloTipoAsig",
                OrdenTipoAsignatura = 1,
                NombreTipoAsignatura = "Básico",
                IdRefCurso = "ref",
                NumeroCurso = 1,
                AnyoAcademicoInicio = 2020,
                AnyoAcademicoFin = 2022,
                PeriodoLectivo = "2022-1",
                DuracionPeriodo = "6 meses",
                SimboloDuracionPeriodo = "&",
                IdRefIdiomaImparticion = "idioma",
                SimboloIdiomaImparticion = "string",
                Reconocida = true,
                SituacionAsignaturaId = 1,
                ExpedienteAlumnoId = 1,
                AsignaturaExpedienteId = 0
            };


            var asignaturaExpendiente = new AsignaturaExpediente
            {
                IdRefAsignaturaPlan = null,
                NombreAsignatura = "Matemática I",
                CodigoAsignatura = "UNIRM_01",
                OrdenAsignatura = 1,
                Ects = 1,
                IdRefTipoAsignatura = "refTipoAsig",
                SimboloTipoAsignatura = "simboloTipoAsig",
                OrdenTipoAsignatura = 1,
                NombreTipoAsignatura = "Básico",
                IdRefCurso = "ref",
                NumeroCurso = 1,
                AnyoAcademicoInicio = 2020,
                AnyoAcademicoFin = 2022,
                PeriodoLectivo = "2022-1",
                DuracionPeriodo = "6 meses",
                SimboloDuracionPeriodo = "&",
                IdRefIdiomaImparticion = "idioma",
                SimboloIdiomaImparticion = "string",
                Reconocida = true
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };

            var sut = new Mock<CreateAsignaturaExpedienteCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.AssignNewAsignaturaExpendiente(It.IsAny<CreateAsignaturaExpedienteCommand>()))
                .Returns(asignaturaExpendiente);

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);
            await Context.SaveChangesAsync();

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "247260",
                IdRefPlan = "1",
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            //ACT
            await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.AssignNewAsignaturaExpendiente(It.IsAny<CreateAsignaturaExpedienteCommand>()), Times.Once);
            Assert.Equal(1, Context.AsignaturasExpedientes.Count());
        }

        #endregion

        #region AssignNewAsignaturaExpendiente

        [Fact(DisplayName = "Cuando se asignan los datos enviados del request Retorna la entidad")]
        public async Task AssignNewAsignaturaExpendiente_Ok()
        {
            //ARRANGE
            var request = new CreateAsignaturaExpedienteCommand
            {
                IdRefAsignaturaPlan = null,
                NombreAsignatura = "Matemática I",
                CodigoAsignatura = "UNIRM_01",
                OrdenAsignatura = 1,
                Ects = 1,
                IdRefTipoAsignatura = "refTipoAsig",
                SimboloTipoAsignatura = "simboloTipoAsig",
                OrdenTipoAsignatura = 1,
                NombreTipoAsignatura = "Básico",
                IdRefCurso = "ref",
                NumeroCurso = 1,
                AnyoAcademicoInicio = 2020,
                AnyoAcademicoFin = 2022,
                PeriodoLectivo = "2022-1",
                DuracionPeriodo = "6 meses",
                SimboloDuracionPeriodo = "&",
                IdRefIdiomaImparticion = "idioma",
                SimboloIdiomaImparticion = "string",
                Reconocida = true,
                SituacionAsignaturaId = 1,
                ExpedienteAlumnoId = 1,
                AsignaturaExpedienteId = 0
            };

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };

            var sut = new CreateAsignaturaExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var actual = sut.AssignNewAsignaturaExpendiente(request);

            //ASSERT
            Assert.IsType<AsignaturaExpediente>(actual);
        }

        #endregion

        #region ValidateProperties

        [Fact(DisplayName = "Cuando cumple las validaciones Retorna Ok")]
        public async Task ValidateProperties_Ok()
        {
            //ARRANGE
            var request = new CreateAsignaturaExpedienteCommand
            {
                SituacionAsignaturaId = 1,
                ExpedienteAlumnoId = 1
            };

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);
            await Context.SaveChangesAsync();

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "247260",
                IdRefPlan = "1",
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var asignaturaExpendiente = new Mock<AsignaturaExpediente> { CallBase = true };
            asignaturaExpendiente.SetupAllProperties();
            asignaturaExpendiente.Setup(x =>
                x.VerificarPropiedadesRequeridosParaCrear()).Returns(new List<string>());

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };
            var sut = new CreateAsignaturaExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            await sut.ValidateProperties(asignaturaExpendiente.Object, request, CancellationToken.None);

            //ASSERT
            asignaturaExpendiente.Verify(x =>
                x.VerificarPropiedadesRequeridosParaCrear(), Times.Once);
        }

        [Fact(DisplayName = "Cuando no existe la situación asignatura Retorna NotFound")]
        public async Task ValidateProperties_situacionAsignatura_NotFound()
        {
            //ARRANGE
            var request = new CreateAsignaturaExpedienteCommand
            {
                SituacionAsignaturaId = 2,
                ExpedienteAlumnoId = 1
            };

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);
            await Context.SaveChangesAsync();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };
            var sut = new CreateAsignaturaExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no existe expediente alumno Retorna NotFound")]
        public async Task ValidateProperties_expedienteAlumno_NotFound()
        {
            //ARRANGE
            var request = new CreateAsignaturaExpedienteCommand
            {
                SituacionAsignaturaId = 1,
                ExpedienteAlumnoId = 2
            };

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);
            await Context.SaveChangesAsync();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };
            var sut = new CreateAsignaturaExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = (NotFoundException)await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando no cumple los campos requeridos Retorna ValidationErrors")]
        public async Task ValidateProperties_requeridos_ValidationErrors()
        {
            var request = new CreateAsignaturaExpedienteCommand
            {
                SituacionAsignaturaId = 1,
                ExpedienteAlumnoId = 1
            };

            var situacionAsignatura = new SituacionAsignatura
            {
                Id = 1,
                Nombre = "Matriculada"
            };
            await Context.SituacionesAsignaturas.AddAsync(situacionAsignatura);
            await Context.SaveChangesAsync();

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "247260",
                IdRefPlan = "1",
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateAsignaturaExpedienteCommandHandler>> { CallBase = true };
            var sut = new CreateAsignaturaExpedienteCommandHandler(Context, mockIStringLocalizer.Object);

            var asignaturaExpendiente = new Mock<AsignaturaExpediente> { CallBase = true };
            asignaturaExpendiente.SetupAllProperties();
            asignaturaExpendiente.Setup(x =>
                x.VerificarPropiedadesRequeridosParaCrear()).Returns(new List<string> { "Error1", "Error2" });

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.ValidateProperties(asignaturaExpendiente.Object, request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<ValidationErrorsException>(ex);
        }
        #endregion
    }
}
