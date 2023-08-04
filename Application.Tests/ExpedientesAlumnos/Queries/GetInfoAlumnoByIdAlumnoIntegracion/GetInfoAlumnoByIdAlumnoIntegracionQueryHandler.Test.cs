using System;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Commons;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using System.Collections.Generic;
using AutoMapper;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion
{
    [Collection("CommonTestCollection")]
    public class GetInfoAlumnoByIdAlumnoIntegracionQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetInfoAlumnoByIdAlumnoIntegracionQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando existen registros Devuelve elementos")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 1)
                .Select(e => new ExpedienteAlumno
                {
                    Id = e,
                    IdRefIntegracionAlumno = e.ToString(),
                    IdRefUniversidad = e.ToString()
                }));
            await Context.SaveChangesAsync(CancellationToken.None);

            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient.Setup(s => s.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(new AlumnoAcademicoModel { Persona = new PersonaErpAcademicoModel() });
            mockErpAcademicoServiceClient.Setup(s => s.GetFotoAlumnoById(It.IsAny<int>()))
                .ReturnsAsync(Guid.NewGuid().ToString());
            mockErpAcademicoServiceClient.Setup(s => s.GetUniversidadById(It.IsAny<int>()))
                .ReturnsAsync(new UniversidadAcademicoModel());
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient> { CallBase = true };

            var sut = new Mock<GetInfoAlumnoByIdAlumnoIntegracionQueryHandler>(Context,
                mockErpAcademicoServiceClient.Object, mockCommonsServiceClient.Object, _mapper)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(), It.IsAny<AlumnoAcademicoModel>(), 
                It.IsAny<UniversidadAcademicoModel>(), It.IsAny<List<ExpedienteAlumno>>()))
                    .ReturnsAsync(new AlumnoInfoDto());

            //ACT
            var actual = await sut.Object.Handle(new GetInfoAlumnoByIdAlumnoIntegracionQuery("1"), CancellationToken.None);

            //ASSERT
            sut.Verify(s => s.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(), It.IsAny<AlumnoAcademicoModel>(), 
                It.IsAny<UniversidadAcademicoModel>(), It.IsAny<List<ExpedienteAlumno>>()),
                Times.Once);
            mockErpAcademicoServiceClient.Verify(s => s.GetUniversidadById(It.IsAny<int>()), Times.Once);
            mockErpAcademicoServiceClient.Verify(s => s.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Once);
            mockErpAcademicoServiceClient.Verify(s => s.GetFotoAlumnoById(It.IsAny<int>()), Times.Once);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "Cuando no existen registros en expedientes Devuelve error")]
        public async Task Handle_NotFound()
        {
            //ARRANGE
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, 1)
                .Select(e => new ExpedienteAlumno
                {
                    Id = e,
                    IdRefIntegracionAlumno = e.ToString(),
                    IdRefUniversidad = e.ToString()
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetInfoAlumnoByIdAlumnoIntegracionQuery("456");

            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            mockErpAcademicoServiceClient.Setup(s => s.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(new AlumnoAcademicoModel { Persona = new PersonaErpAcademicoModel() });
            mockErpAcademicoServiceClient.Setup(s => s.GetFotoAlumnoById(It.IsAny<int>()))
                .ReturnsAsync(Guid.NewGuid().ToString());
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient> { CallBase = true };

            var sut = new Mock<GetInfoAlumnoByIdAlumnoIntegracionQueryHandler>(Context,
                mockErpAcademicoServiceClient.Object, mockCommonsServiceClient.Object, _mapper)
            {
                CallBase = true
            };
            sut.Setup(s => s.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(), It.IsAny<AlumnoAcademicoModel>(), 
                It.IsAny<UniversidadAcademicoModel>(), It.IsAny<List<ExpedienteAlumno>>()))
                .ReturnsAsync(new AlumnoInfoDto());

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdAlumnoIntegracion).Message,
                exception.Message);
            sut.Verify(s => s.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(), It.IsAny<AlumnoAcademicoModel>(), 
                It.IsAny<UniversidadAcademicoModel>(), It.IsAny<List<ExpedienteAlumno>>()),
                Times.Never);
            mockErpAcademicoServiceClient.Verify(s => s.GetUniversidadById(It.IsAny<int>()), Times.Never);
            mockErpAcademicoServiceClient.Verify(s => s.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Never);
            mockErpAcademicoServiceClient.Verify(s => s.GetFotoAlumnoById(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region GetAlumnoInfo

        [Fact(DisplayName = "Cuando se envía los parámetros Devuelve el Dto seteado")]
        public async Task GetAlumnoInfo_Ok()
        {
            //ARRANGE
            var universidad = new UniversidadAcademicoModel { IdIntegracion = Guid.NewGuid().ToString() };
            var expedienteAlumno = new ExpedienteAlumno
            {
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                AlumnoApellido2 = Guid.NewGuid().ToString(),
                AlumnoEmail = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString()
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                expedienteAlumno
            };
            var alumnoAcademicoModel = new AlumnoAcademicoModel
            {
                Persona = new PersonaErpAcademicoModel
                {
                    Celular = Guid.NewGuid().ToString(),
                    FechaNacimiento = DateTime.Now,
                    Foto = Guid.NewGuid().ToString(),
                    IdRefPaisNacionalidad = Guid.NewGuid().ToString(),
                    Sexo = Guid.NewGuid().ToString()
                },
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new()
                    {
                        Id = 1,
                        DisplayName = Guid.NewGuid().ToString(),
                        Tipo = new TipoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        RegionEstudio = new RegionEstudioAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        PlanOfertado = new PlanOfertadoDtoAcademicoModel
                        {
                            PeriodoAcademico = new PeriodoAcademicoAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                                AnyoAcademico = new AnyoAcademicoAcademicoModel
                                {
                                    DisplayName = Guid.NewGuid().ToString(),
                                }
                            },
                            Plan = new PlanAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                            }
                        }
                    },
                    new()
                    {
                        Id = 2,
                        DisplayName = Guid.NewGuid().ToString(),
                        Tipo = new TipoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        RegionEstudio = new RegionEstudioAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        PlanOfertado = new PlanOfertadoDtoAcademicoModel
                        {
                            PeriodoAcademico = new PeriodoAcademicoAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                                AnyoAcademico = new AnyoAcademicoAcademicoModel
                                {
                                    DisplayName = Guid.NewGuid().ToString(),
                                }
                            },
                            Plan = new PlanAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                            }
                        }
                    }
                }
            };
            var nacionalidad = Guid.NewGuid().ToString();

            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient> { CallBase = true };
            
            mockCommonsServiceClient.Setup(s => s.GetCountry(It.IsAny<string>()))
                .ReturnsAsync(new CountryCommonsModel { Name = nacionalidad });
            var sut = new GetInfoAlumnoByIdAlumnoIntegracionQueryHandler(Context, mockErpAcademicoServiceClient.Object,
                mockCommonsServiceClient.Object, _mapper);

            //ACT
            var actual = await sut.GetAlumnoInfo(expedienteAlumno, alumnoAcademicoModel, universidad, expedientes);

            //ASSERT
            mockCommonsServiceClient.Verify(s => s.GetCountry(It.IsAny<string>()), Times.Once);
            Assert.Equal(alumnoAcademicoModel.Persona.Celular, actual.Celular);
            Assert.Equal(alumnoAcademicoModel.Persona.FechaNacimiento, actual.FechaNacimiento);
            Assert.Equal(alumnoAcademicoModel.Persona.Foto, actual.Foto);
            Assert.Equal(nacionalidad, actual.Nacionalidad);
            Assert.Equal(alumnoAcademicoModel.Persona.Sexo, actual.Sexo);
            Assert.Equal(alumnoAcademicoModel.Id, actual.IdAlumno);
            Assert.Equal($"{expedienteAlumno.AlumnoNombre} {expedienteAlumno.AlumnoApellido1} {expedienteAlumno.AlumnoApellido2}", actual.DisplayName);
            Assert.Equal(expedienteAlumno.AlumnoEmail, actual.Email);
            Assert.Equal(expedienteAlumno.IdRefTipoDocumentoIdentificacionPais, actual.TipoDocumentoIdentificacionPais);
            Assert.Equal(expedienteAlumno.AlumnoNroDocIdentificacion, actual.NroDocIdentificacion);
            Assert.Equal(expedienteAlumno.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(universidad.IdIntegracion, actual.IdUniversidadIntegracion);
            Assert.Equal(expedientes.Count, actual.Expedientes.Count);
            Assert.Equal(alumnoAcademicoModel.Matriculas.Count, actual.Matriculas.Count);
        }

        [Fact(DisplayName = "Cuando se envía los parámetros y no tiene nacionalidad Devuelve el Dto seteado con nacionalidad null")]
        public async Task GetAlumnoInfo_SinNacionalidad()
        {
            //ARRANGE
            var universidad = new UniversidadAcademicoModel { IdIntegracion = Guid.NewGuid().ToString() };
            var expedienteAlumno = new ExpedienteAlumno
            {
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                AlumnoApellido2 = Guid.NewGuid().ToString(),
                AlumnoEmail = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString()
            };
            var expedientes = new List<ExpedienteAlumno>
            {
                expedienteAlumno
            };
            var alumnoAcademicoModel = new AlumnoAcademicoModel
            {
                Persona = new PersonaErpAcademicoModel
                {
                    Celular = Guid.NewGuid().ToString(),
                    FechaNacimiento = DateTime.Now,
                    Foto = Guid.NewGuid().ToString(),
                    Sexo = Guid.NewGuid().ToString()
                },
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new()
                    {
                        Id = 1,
                        DisplayName = Guid.NewGuid().ToString(),
                        Tipo = new TipoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        RegionEstudio = new RegionEstudioAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString(),
                        },
                        PlanOfertado = new PlanOfertadoDtoAcademicoModel
                        {
                            PeriodoAcademico = new PeriodoAcademicoAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                                AnyoAcademico = new AnyoAcademicoAcademicoModel
                                {
                                    DisplayName = Guid.NewGuid().ToString(),
                                }
                            },
                            Plan = new PlanAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                            }
                        }
                    }
                }
            };
            var nacionalidad = Guid.NewGuid().ToString();

            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient> { CallBase = true };
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient> { CallBase = true };

            mockCommonsServiceClient.Setup(s => s.GetCountry(It.IsAny<string>()))
                .ReturnsAsync(new CountryCommonsModel { Name = nacionalidad });
            var sut = new GetInfoAlumnoByIdAlumnoIntegracionQueryHandler(Context, mockErpAcademicoServiceClient.Object,
                mockCommonsServiceClient.Object, _mapper);

            //ACT

            var actual = await sut.GetAlumnoInfo(expedienteAlumno, alumnoAcademicoModel, universidad, expedientes);

            //ASSERT
            mockCommonsServiceClient.Verify(s => s.GetCountry(It.IsAny<string>()), Times.Never);
            Assert.Equal(alumnoAcademicoModel.Persona.Celular, actual.Celular);
            Assert.Equal(alumnoAcademicoModel.Persona.FechaNacimiento, actual.FechaNacimiento);
            Assert.Equal(alumnoAcademicoModel.Persona.Foto, actual.Foto);
            Assert.Null(actual.Nacionalidad);
            Assert.Equal(alumnoAcademicoModel.Persona.Sexo, actual.Sexo);
            Assert.Equal(alumnoAcademicoModel.Id, actual.IdAlumno);
            Assert.Equal($"{expedienteAlumno.AlumnoNombre} {expedienteAlumno.AlumnoApellido1} {expedienteAlumno.AlumnoApellido2}", actual.DisplayName);
            Assert.Equal(expedienteAlumno.AlumnoEmail, actual.Email);
            Assert.Equal(expedienteAlumno.IdRefTipoDocumentoIdentificacionPais, actual.TipoDocumentoIdentificacionPais);
            Assert.Equal(expedienteAlumno.AlumnoNroDocIdentificacion, actual.NroDocIdentificacion);
            Assert.Equal(expedienteAlumno.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(universidad.IdIntegracion, actual.IdUniversidadIntegracion);
            Assert.Equal(expedientes.Count, actual.Expedientes.Count);
            Assert.Equal(alumnoAcademicoModel.Matriculas.Count, actual.Matriculas.Count);
        }

        #endregion
    }
}
