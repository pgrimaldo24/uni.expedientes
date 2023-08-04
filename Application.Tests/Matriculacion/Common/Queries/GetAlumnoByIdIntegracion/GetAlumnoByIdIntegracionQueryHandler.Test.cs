using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion
{
    [Collection("CommonTestCollection")]
    public class GetAlumnoByIdIntegracionQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new GetAlumnoByIdIntegracionQuery("1", "1");

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler>> { CallBase = true };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<GetAlumnoByIdIntegracionQueryHandler>(Context, mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "1"
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "2"
                    }
                }
            };

            mockIErpAcademicoServiceClient.Setup(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(alumnoAcademico);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString()
            };

            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.NotNull(actual.ExpedienteAlumno);
            Assert.NotNull(actual.AlumnoAcademicoModel);
            Assert.NotNull(actual.MatriculaAcademicoModel);
        }

        [Fact(DisplayName = "Cuando el alumno no existe en erp académico Retorna BadRequest")]
        public async Task AlumnoAcademico_BadRequest()
        {
            //ARRANGE
            const string mensajeEsperado = "El Alumno no existe en erp académico";

            var request = new GetAlumnoByIdIntegracionQuery("1", "10");

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler>> { CallBase = true };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetAlumnoByIdIntegracionQueryHandler>(Context, mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando matrícula existe Retorna BadRequest")]
        public async Task Matricula_BadRequest()
        {
            //ARRANGE
            const string mensajeEsperado = "La matrícula no existe";

            var request = new GetAlumnoByIdIntegracionQuery("1", "10");

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler>> { CallBase = true };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetAlumnoByIdIntegracionQueryHandler>(Context, mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        }
                    },
                    new() {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        }
                    }
                }
            };

            mockIErpAcademicoServiceClient.Setup(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(alumnoAcademico);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando expediente alumno no existe Retorna BadRequest")]
        public async Task Expediente_alumno_BadRequest()
        {
            //ARRANGE
            const string mensajeEsperado = "El expediente de alumno no existe";

            var request = new GetAlumnoByIdIntegracionQuery("1", "1");

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler>> { CallBase = true };

            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<GetAlumnoByIdIntegracionQueryHandler>(Context, mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        IdIntegracion = "1",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "1"
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "2"
                    }
                }
            };

            mockIErpAcademicoServiceClient.Setup(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(alumnoAcademico);

            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 3,
                IdRefIntegracionAlumno = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString()
            };

            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
