using MediatR;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon
{
    [Collection("CommonTestCollection")]
    public class EliminarConsolidacionRequisitosAnuladaCommonCommandHandlerTest : TestBase
    {
        #region Handle
        [Fact(DisplayName = "Cuando se elimina consolidaciones Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>
                {
                    new ()
                    {
                        RequisitoExpedienteId = 1,
                    },
                    new ()
                    {
                        RequisitoExpedienteId = 2,
                    },
                    new ()
                    {
                        RequisitoExpedienteId = 3,
                    }
                }
            };

            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Matriculas = new List<MatriculaAcademicoModel>
                {
                    new () {
                        Id = 1,
                        AsignaturaMatriculadas = new List<AsignaturaMatriculadaModel>
                        {
                            new ()
                            {
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 1,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 1,
                                        Asignatura = new AsignaturaErpAcademicoModel
                                        {
                                            Id = 1,
                                            TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                                            {
                                                Id = 1,
                                                Nombre = "Básica"
                                            }
                                        }
                                    }
                                }
                            },
                            new ()
                            {
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 2,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel()
                                    {
                                        Id = 2,
                                        Asignatura = new AsignaturaErpAcademicoModel
                                        {
                                            Id = 2,
                                            TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                                            {
                                                Id = 2,
                                                Nombre = "Trabajo fin de Grado"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new () {
                        Id = 2,
                        IdIntegracion = "2",
                        Estado = new EstadoMatriculaAcademicoModel
                        {
                            EsAlta = false
                        },
                        IdRefExpedienteAlumno = "2",
                        FechaCambioEstado = DateTime.UtcNow,
                        AsignaturaMatriculadas = new List<AsignaturaMatriculadaModel>
                        {
                            new ()
                            {
                                AsignaturaOfertada = new AsignaturaOfertadaModel
                                {
                                    Id = 1,
                                    AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                                    {
                                        Id = 1,
                                        Asignatura = new AsignaturaErpAcademicoModel
                                        {
                                           Id = 3,
                                           TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                                           {
                                               Id = 4,
                                               Nombre = "Trabajo fin de Máster"
                                           }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var IdsAsignaturasOfertadas = new List<int> { 5, 6, 7 };

            var alumnoRequest = new AlumnoMatricula
            {
                ExpedienteAlumno = expedienteAlumno,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = alumnoAcademico.Matriculas.First()
            };

            var nivelUsoComportamiento = new List<NivelUsoComportamientoExpediente>
            {
                new ()
                {
                    Id = 30,
                    IdRefUniversidad = "1",
                    AcronimoUniversidad = "UNIR",
                    IdRefTipoEstudio = "12",
                    NombreTipoEstudio = "Máster Universitario",
                    IdRefEstudio = "2218",
                    NombreEstudio = "Máster Universitario..",
                    IdRefPlan = "3827",
                    NombrePlan = "Máster Universitario..",
                    IdRefTipoAsignatura = "5",
                    NombreTipoAsignatura = "Obligatorias",
                    IdRefAsignaturaPlan = "21979",
                    IdRefAsignatura = "15357",
                    NombreAsignatura = "Abordaje Nutricional Basado en el Metabolismo",
                    TipoNivelUsoId = 6,
                    ComportamientoExpedienteId = 1
                }
            };

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                Nombre = "Comportamiento 1",
                Descripcion = string.Empty,
                EstaVigente = true,
                Bloqueado = false,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new ()
                    {
                        Id = 1,
                        ComportamientoExpedienteId = 1,
                        RequisitoExpedienteId = 1,
                        TipoRequisitoExpedienteId = 1
                    }
                }
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamientoExpediente);
            await Context.SaveChangesAsync();

            var mock = new Mock<EliminarConsolidacionRequisitosAnuladaCommonCommandHandler>(Context)
            { CallBase = true };

            mock.Setup(s => s.GetNivelesUsoComportamientos(It.IsAny<List<AsignaturaExpediente>>()))
                .Returns(nivelUsoComportamiento);

            //ACT
            await mock.Object.Handle(new EliminarConsolidacionRequisitosAnuladaCommonCommand(IdsAsignaturasOfertadas, alumnoRequest), CancellationToken.None);

            //ASSERT
            mock.Verify(s => s.GetNivelesUsoComportamientos(It.IsAny<List<AsignaturaExpediente>>()), Times.Once);
        }
        #endregion

        #region GetNivelesUsoComportamientos
        [Fact(DisplayName = "Cuando obtiene niveles de uso comportamiento Retorna Lista")]
        public async Task GetNivelesUsoComportamiento()
        {
            //ARRANGE
            var request = new List<AsignaturaExpediente>
            {
                new ()
                {
                    Id = 667,
                    IdRefAsignaturaPlan = "6647",
                    NombreAsignatura = "Medición, Investigación e Innovación Educativa",
                    CodigoAsignatura = "738",
                    OrdenAsignatura = 2,
                    Ects = 6,
                    IdRefTipoAsignatura = "5",
                    SituacionAsignaturaId = 1,
                    ExpedienteAlumnoId = 204889
                },
                new ()
                {
                    Id = 668,
                    IdRefAsignaturaPlan = "6648",
                    NombreAsignatura = "Atención Psicoeducativa...",
                    CodigoAsignatura = "739",
                    OrdenAsignatura = 3,
                    Ects = 6,
                    IdRefTipoAsignatura = "6",
                    SituacionAsignaturaId = 1,
                    ExpedienteAlumnoId = 204889
                }
            };

            var mock = new Mock<EliminarConsolidacionRequisitosAnuladaCommonCommandHandler>(Context)
            { CallBase = true };

            var tipoNivelUsoTipoAsignatura = new TipoNivelUso
            {
                Id = 5,
                Nombre = "Tipo de Asignatura",
                Orden = 5
            };

            var tipoNivelUsoAsignaturaPlan = new TipoNivelUso
            {
                Id = 6,
                Nombre = "Asignatura Plan",
                Orden = 6
            };

            await Context.TiposNivelesUso.AddAsync(tipoNivelUsoTipoAsignatura);
            await Context.TiposNivelesUso.AddAsync(tipoNivelUsoAsignaturaPlan);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                Nombre = "Comportamiento 1",
                Descripcion = string.Empty,
                EstaVigente = true,
                Bloqueado = false
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamientoExpediente);
            await Context.SaveChangesAsync();

            var nivelUsoTipoAsignatura1 = new NivelUsoComportamientoExpediente
            {
                Id = 1,
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = "5",
                IdRefAsignaturaPlan = "100",
                TipoNivelUso = tipoNivelUsoTipoAsignatura,
                ComportamientoExpediente = comportamientoExpediente
            };

            var nivelUsoTipoAsignatura2 = new NivelUsoComportamientoExpediente
            {
                Id = 2,
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = "5",
                IdRefAsignaturaPlan = "6648",
                TipoNivelUso = tipoNivelUsoAsignaturaPlan,
                ComportamientoExpediente = comportamientoExpediente
            };

            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelUsoTipoAsignatura1);
            await Context.NivelesUsoComportamientosExpedientes.AddAsync(nivelUsoTipoAsignatura2);
            await Context.SaveChangesAsync();

            //ACT
            var actual = mock.Object.GetNivelesUsoComportamientos(request);

            //ASSERT
            Assert.IsAssignableFrom<IEnumerable<NivelUsoComportamientoExpediente>>(actual);
            Assert.True(actual.Any());
        }
        #endregion
    }
}
