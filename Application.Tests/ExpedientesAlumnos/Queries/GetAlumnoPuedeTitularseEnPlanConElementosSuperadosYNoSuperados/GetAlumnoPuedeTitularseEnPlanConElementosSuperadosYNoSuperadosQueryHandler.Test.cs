using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    [Collection("CommonTestCollection")]
    public class GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando se obtiene los elementos superados y no superados Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            const string idRefIntegracionAlumno = "123456";
            const string idRefPlan = "44";

            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idRefPlan
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefUniversidad = "3",
                IdRefPlan = idRefPlan
            };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = new ExpedienteExpedientesIntegrationModel
                {
                    Asignaturas = Enumerable.Range(1, 3).Select(i =>
                        new AsignaturaErpAcademicoExpedientesIntegrationModel
                        {
                            IdAsignatura = i
                        }).ToList(),
                    Bloqueado = true,
                    Bloqueos = Enumerable.Range(1, 3).Select(i => new BloqueosIntegrationModel
                    {
                        Nombre = $"Bloqueo{i}"
                    }).ToList()
                }
            };
            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                    s.GetExpedienteGestorFormatoErpWithAsignaturas(It.Is<string>(i => i == idRefIntegracionAlumno),
                        It.Is<int>(i => i == int.Parse(idRefPlan))))
                .ReturnsAsync(expedienteGestor);

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(s =>
                    s.GetAsignaturasPlanesParaTitulacion(It.IsAny<AsignaturasPlanTitulacionParameters>()))
                .ReturnsAsync(new List<AsignaturaPlanTitulacionErpAcademico>());

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, 3).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                                {
                                    Id = i,
                                    DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        ErroresReconocimientosOrigen = Enumerable.Range(1, 3)
                                            .Select(e => $"{Guid.NewGuid()}{e}").ToList()
                                    }
                                }
                            }).ToList()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = Enumerable.Range(1, 3).Select(i =>
                        new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                        {
                            Id = i,
                            Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                                {
                                    ErroresReconocimientosOrigen = Enumerable.Range(1, 3)
                                        .Select(e => $"{Guid.NewGuid()}{e}").ToList()
                                }
                            }
                        }).ToList()
                }
            };
            sut.Setup(s =>
                    s.GetAsignaturasPlanes(It.IsAny<ExpedienteAlumno>(),
                        It.IsAny<ExpedienteExpedientesIntegrationModel>()))
                .ReturnsAsync(alumnoPuedeTitularseDto);

            sut.Setup(s => s.SetAsignaturasConIntegracionErpAcademico(
                It.IsAny<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel>(),
                It.IsAny<List<AsignaturaPlanTitulacionErpAcademico>>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()));
            sut.Setup(s => s.SetExpedicion(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.Is<int>(i => i == int.Parse(idRefPlan))))
                .Returns(Task.CompletedTask);
            sut.Setup(s => s.SetCreditosObtenidosAsignaturasSuperadas(It.IsAny<AlumnoPuedeTitularseDto>()));
            sut.Setup(s => s.SetArcosSuperados(It.IsAny<AlumnoPuedeTitularseDto>()));
            sut.Setup(s =>
                s.SetBloqueos(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<List<BloqueosIntegrationModel>>()));

            var validacionAlumnoMatriculacion = new ValidateAlumnoMatriculacionErpAcademicoModel
            {
                CausasFallosMatriculas = Enumerable.Range(1, 3).Select(i => $"{Guid.NewGuid()}{i}").ToList(),
                MatriculasOk = false
            };
            sut.Setup(s =>
                s.GetCausasFallosComprobacionMatriculasDocumentacionErp(It.Is<string>(i => i == idRefIntegracionAlumno),
                    It.Is<string>(i => i == idRefPlan))).ReturnsAsync(validacionAlumnoMatriculacion);
            sut.Setup(s => s.FiltrarAsignaturasExpedienteAlumno(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery>()));

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s =>
                s.GetAsignaturasPlanesParaTitulacion(It.IsAny<AsignaturasPlanTitulacionParameters>()), Times.Once);
            sut.Verify(s =>
                s.GetAsignaturasPlanes(It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<ExpedienteExpedientesIntegrationModel>()), Times.Once);
            sut.Verify(s => s.SetAsignaturasConIntegracionErpAcademico(
                It.IsAny<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel>(),
                It.IsAny<List<AsignaturaPlanTitulacionErpAcademico>>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()), Times.Exactly(6));
            sut.Verify(s => s.SetExpedicion(It.IsAny<AlumnoPuedeTitularseDto>(),
                    It.IsAny<ExpedienteExpedientesIntegrationModel>(), It.Is<int>(i => i == int.Parse(idRefPlan))),
                Times.Once);
            sut.Verify(s => s.SetCreditosObtenidosAsignaturasSuperadas(It.IsAny<AlumnoPuedeTitularseDto>()),
                Times.Once);
            sut.Verify(s => s.SetArcosSuperados(It.IsAny<AlumnoPuedeTitularseDto>()), Times.Once);
            sut.Verify(s =>
                    s.SetBloqueos(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<List<BloqueosIntegrationModel>>()),
                Times.Once);
            sut.Verify(s =>
                s.GetCausasFallosComprobacionMatriculasDocumentacionErp(It.Is<string>(i => i == idRefIntegracionAlumno),
                    It.Is<string>(i => i == idRefPlan)), Times.Once);
            sut.Verify(s => s.FiltrarAsignaturasExpedienteAlumno(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery>()));
            mockIExpedientesGestorUnirServiceClient.Verify(s =>
                s.GetExpedienteGestorFormatoErpWithAsignaturas(It.Is<string>(i => i == idRefIntegracionAlumno),
                    It.Is<int>(i => i == int.Parse(idRefPlan))), Times.Once);
            Assert.NotNull(actual);
            Assert.Equal(validacionAlumnoMatriculacion.MatriculasOk, actual.MatriculasOk);
            Assert.Equal(validacionAlumnoMatriculacion.CausasFallosMatriculas.Count,
                actual.CausasFalloMatriculas.Count);
        }

        [Fact(DisplayName = "Cuando no existe el expediente Devuelve error")]
        public async Task Handle_NoExpediente()
        {
            //ARRANGE
            var request = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                IdRefIntegracionAlumno = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString()
            };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };
            var mensajeEsperado =
                $"No se ha encontrado un Expediente con el IdPlan {request.IdRefPlan} y IdIntegracionAlumno {request.IdRefIntegracionAlumno}.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando en el expediente gestor Devuelve error")]
        public async Task Handle_NoExpedienteGestor()
        {
            //ARRANGE
            const string idRefIntegracionAlumno = "123456";
            const string idRefPlan = "44";

            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idRefPlan
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idRefPlan
            };

            var erroresExpedienteGestor = Enumerable.Range(1, 2).Select(i => $"{Guid.NewGuid()}{i}").ToList();

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };
            var mensajeEsperado =
                $"[Expedientes Gestor]: { string.Join(", ", erroresExpedienteGestor)}.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Errors = erroresExpedienteGestor
            };

            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                    s.GetExpedienteGestorFormatoErpWithAsignaturas(It.Is<string>(i => i == idRefIntegracionAlumno),
                        It.Is<int>(i => i == int.Parse(idRefPlan))))
                .ReturnsAsync(expedienteGestor);

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando en el expediente gestor no devuelve asignaturas Devuelve error")]
        public async Task Handle_SinAsignaturas()
        {
            //ARRANGE
            const string idRefIntegracionAlumno = "123456";
            const string idRefPlan = "44";

            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idRefPlan
            });
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                IdRefIntegracionAlumno = idRefIntegracionAlumno,
                IdRefPlan = idRefPlan
            };

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };
            const string mensajeEsperado = "No se puede localizar las Asignaturas en el Expediente del Gestor Unir.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var expedienteGestor = new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = new ExpedienteExpedientesIntegrationModel
                {
                    Asignaturas = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>()
                }
            };

            mockIExpedientesGestorUnirServiceClient.Setup(s =>
                    s.GetExpedienteGestorFormatoErpWithAsignaturas(It.Is<string>(i => i == idRefIntegracionAlumno),
                        It.Is<int>(i => i == int.Parse(idRefPlan))))
                .ReturnsAsync(expedienteGestor);

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Contains(mensajeEsperado, ex.Message);
        }

        #endregion

        #region GetAsignaturasPlanes

        [Fact(DisplayName = "Cuando se obtiene las asignaturas planes Devuelve ok")]
        public async Task GetAsignaturasPlanes_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "45"
            };

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var expedienteGestorUnir = new ExpedienteExpedientesIntegrationModel
            {
                Asignaturas = Enumerable.Range(0, 3).Select(i => new AsignaturaErpAcademicoExpedientesIntegrationModel
                {
                    IdAsignatura = i - 1,
                    Superado = i % 2 == 0
                }).ToList()
            };

            sut.Setup(s =>
                s.RemoverAsignaturasReconocidas(It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()));
            sut.Setup(s => s.GetPlanSurpassedErp(It.IsAny<int>(), It.IsAny<ExpedienteAlumno>(),
                    It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()))
                .ReturnsAsync(new ExpedienteAlumnoTitulacionPlanDto());

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 3).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                AsignaturasBloqueSuperadas = Enumerable.Range(1, 3).Select(y =>
                                    new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        IdAsignaturaPlan = y
                                    }).ToList()
                            }).ToList()
                    }
                }
            };
            sut.Setup(s => s.ConvertToExpedienteAlumno(It.IsAny<ExpedienteAlumnoTitulacionPlanDto>()))
                .Returns(alumnoPuedeTitularseDto);

            sut.Setup(s => s.SetAsignaturasReconocidas(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()));
            sut.Setup(s => s.SetElementosNoSuperados(It.IsAny<int[]>(),
                It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                It.IsAny<AlumnoPuedeTitularseDto>()));
            sut.Setup(s => s.RemoverAsignaturasNoSuperadas(It.IsAny<int[]>(), It.IsAny<AlumnoPuedeTitularseDto>()));
            sut.Setup(s =>
                s.RemoverBloquesAsignaturasNoSuperadas(
                    It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<AlumnoPuedeTitularseDto>()));
            sut.Setup(s =>
                s.RemoverSubBloquesAsignaturasNoSuperadas(It.IsAny<int[]>(), It.IsAny<AlumnoPuedeTitularseDto>()));

            //ACT
            var actual = await sut.Object.GetAsignaturasPlanes(expedienteAlumno, expedienteGestorUnir);

            //ASSERT
            sut.Verify(s => s.ConvertToExpedienteAlumno(It.IsAny<ExpedienteAlumnoTitulacionPlanDto>()), Times.Once);
            sut.Verify(s => s.SetAsignaturasReconocidas(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()), Times.Once);
            sut.Verify(s => s.SetElementosNoSuperados(It.IsAny<int[]>(),
                It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                It.IsAny<AlumnoPuedeTitularseDto>()), Times.Once);
            sut.Verify(s => s.RemoverAsignaturasNoSuperadas(It.IsAny<int[]>(), It.IsAny<AlumnoPuedeTitularseDto>()),
                Times.Once);
            sut.Verify(s =>
                s.RemoverBloquesAsignaturasNoSuperadas(
                    It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<AlumnoPuedeTitularseDto>()), Times.Once);
            sut.Verify(s =>
                    s.RemoverSubBloquesAsignaturasNoSuperadas(It.IsAny<int[]>(), It.IsAny<AlumnoPuedeTitularseDto>()),
                Times.Once);
            Assert.Equal(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Count,
                actual.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Count);
        }

        [Fact(DisplayName = "Cuando se obtiene las asignaturas planes Devuelve ok")]
        public async Task GetAsignaturasPlanes_SinAsignaturas()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "45"
            };

            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var expedienteGestorUnir = new ExpedienteExpedientesIntegrationModel();

            const string causaInsuperacionEsperada = "El alumno no tiene ninguna Asignatura Superada.";

            //ACT
            var actual = await sut.GetAsignaturasPlanes(expedienteAlumno, expedienteGestorUnir);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Single(actual.EsPlanSuperado.CausasInsuperacion);
            Assert.Equal(causaInsuperacionEsperada, actual.EsPlanSuperado.CausasInsuperacion.First());
        }

        #endregion

        #region RemoverAsignaturasReconocidas

        [Fact(DisplayName = "Cuando se eliminan las asignaturas reconocidas Devuelve ok")]
        public void RemoverAsignaturasReconocidas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var mockAsignaturaReconocida = new Mock<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                CallBase = true
            };
            mockAsignaturaReconocida.SetupAllProperties();
            mockAsignaturaReconocida.Object.IdAsignatura = -1;
            mockAsignaturaReconocida.Setup(s => s.GetAsignaturasReconocidas()).Returns(true);

            var mockAsignaturaNoReconocida = new Mock<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                CallBase = true
            };
            mockAsignaturaNoReconocida.SetupAllProperties();
            mockAsignaturaNoReconocida.Object.IdAsignatura = 1;
            mockAsignaturaNoReconocida.Setup(s => s.GetAsignaturasReconocidas()).Returns(false);

            var expedienteGestorUnir = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                mockAsignaturaReconocida.Object,
                mockAsignaturaNoReconocida.Object
            };

            //ACT
            sut.RemoverAsignaturasReconocidas(expedienteGestorUnir);

            //ASSERT
            Assert.Single(expedienteGestorUnir);
            Assert.DoesNotContain(expedienteGestorUnir, eg => eg.IdAsignatura < 0);
        }

        #endregion

        #region GetPlanSurpassedErpAsync

        [Fact(DisplayName = "Cuando se obtiene el plan superado Devuelve ok")]
        public async Task GetPlanSurpassedErp_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            const int idPlan = 1;
            mockIErpAcademicoServiceClient
                .Setup(s => s.ItIsPlanSurpassed(It.Is<int>(i => i == idPlan), It.IsAny<EsPlanSuperadoParameters>()))
                .ReturnsAsync(new PlanSuperadoErpAcademicoModel());

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturasSuperadas = Enumerable.Range(1, 2).Select(i =>
                new AsignaturaErpAcademicoExpedientesIntegrationModel
                {
                    IdAsignatura = i
                }).ToList();

            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefNodo = "1",
                IdRefVersionPlan = "2"
            };

            //ACT
            var actual = await sut.GetPlanSurpassedErp(idPlan, expedienteAlumno, asignaturasSuperadas);

            //ASSERT
            mockIErpAcademicoServiceClient
                .Verify(s => s.ItIsPlanSurpassed(It.Is<int>(i => i == idPlan), It.IsAny<EsPlanSuperadoParameters>()), Times.Once);
            Assert.NotNull(actual.EsPlanSuperado);
        }

        #endregion

        #region ConvertToExpedienteAlumno

        [Fact(DisplayName = "Cuando se convierte al modelo para el servicio reconocidas Devuelve ok")]
        public void ConvertToExpedienteAlumno_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var asignaturaPlanAlumnoPuedeTitularse = Enumerable.Range(1, 3)
                .Select(i => new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = i
                }).ToList();
            sut.Setup(s => s.ConvertToAsignaturasPlanSuperadas(It.IsAny<List<AsignaturaPlanErpAcademicoModel>>()))
                .Returns(asignaturaPlanAlumnoPuedeTitularse);
            var arcosAlumnoPuedeTitularse = Enumerable.Range(1, 3)
                .Select(i => new ArcoAlumnoPuedeTitulularseErpAcademicoModel
                {
                    Id = i
                }).ToList();
            sut.Setup(s => s.ConvertToArcos(It.IsAny<List<ArcoErpAcademicoModel>>()))
                .Returns(arcosAlumnoPuedeTitularse);
            var bloquesSuperadosAlumnoPuedeTitularse = Enumerable.Range(1, 3)
                .Select(i => new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = i
                }).ToList();
            sut.Setup(s => s.ConvertToBloquesSuperados(It.IsAny<List<BloqueSuperadoErpAcademicoModel>>()))
                .Returns(bloquesSuperadosAlumnoPuedeTitularse);

            var expedienteAlumnoTitulacionPlanDto = new ExpedienteAlumnoTitulacionPlanDto
            {
                EsPlanSuperado = new PlanSuperadoErpAcademicoModel
                {
                    EsSuperado = true,
                    CausasInsuperacion = new List<string>(),
                    ElementosSuperados = new ElementoSuperadoErpAcademicoModel
                    {
                        HitosObtenidos = Enumerable.Range(1, 3).Select(i => new HitoErpAcademicoModel
                        {
                            Id = i
                        }).ToList(),
                        NodosActuales = new List<int> { 1, 2 },
                        NodosAlcanzados = Enumerable.Range(1, 3).Select(i => new NodoErpAcademicoModel
                        {
                            Id = i
                        }).ToList(),
                        RequerimientosSuperados = Enumerable.Range(1, 3).Select(i =>
                            new RequerimientoPlanErpAcademicoModel
                            {
                                Id = i
                            }).ToList(),
                        TrayectosPlanSuperados = Enumerable.Range(1, 3).Select(i =>
                            new TrayectoPlanErpAcademicoModel
                            {
                                Id = i
                            }).ToList()
                    }
                }
            };

            //ACT
            var actual = sut.Object.ConvertToExpedienteAlumno(expedienteAlumnoTitulacionPlanDto);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.EsSuperado, actual.EsPlanSuperado.EsSuperado);
            Assert.Empty(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.CausasInsuperacion);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.ElementosSuperados.HitosObtenidos,
                actual.EsPlanSuperado.ElementosSuperados.HitosObtenidos);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.ElementosSuperados.NodosActuales,
                actual.EsPlanSuperado.ElementosSuperados.NodosActuales);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.ElementosSuperados.NodosAlcanzados,
                actual.EsPlanSuperado.ElementosSuperados.NodosAlcanzados);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.ElementosSuperados.RequerimientosSuperados,
                actual.EsPlanSuperado.ElementosSuperados.RequerimientosSuperados);
            Assert.Equal(expedienteAlumnoTitulacionPlanDto.EsPlanSuperado.ElementosSuperados.TrayectosPlanSuperados,
                actual.EsPlanSuperado.ElementosSuperados.TrayectosPlanSuperados);
            sut.Verify(s => s.ConvertToAsignaturasPlanSuperadas(It.IsAny<List<AsignaturaPlanErpAcademicoModel>>()),
                Times.Once);
            sut.Verify(s => s.ConvertToArcos(It.IsAny<List<ArcoErpAcademicoModel>>()), Times.Once);
            sut.Verify(s => s.ConvertToBloquesSuperados(It.IsAny<List<BloqueSuperadoErpAcademicoModel>>()), Times.Once);
        }

        #endregion

        #region ConvertToAsignaturasPlanSuperadas

        [Fact(DisplayName = "Cuando se convierte al modelo para las asignaturas superadas Devuelve ok")]
        public void ConvertToAsignaturasPlanSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var datoGestorEsperado = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                Convocatoria = Guid.NewGuid().ToString()
            };
            sut.Setup(s => s.ConvertToDatosGestor(It.IsAny<AsignaturaPlanErpAcademicoModel>()))
                .Returns(datoGestorEsperado);

            var elementosSuperados = Enumerable.Range(1, 3).Select(i => new AsignaturaPlanErpAcademicoModel
            {
                Id = i,
                Asignatura = new AsignaturaErpAcademicoModel
                {
                    Id = i,
                    Codigo = $"Codigo {i}",
                    CodigoOficialExterno = Guid.NewGuid().ToString(),
                    Creditos = 5.56d,
                    Curso = i % 2 == 0 ? Guid.NewGuid().ToString() : null,
                    IdentificadorOficialExterno = Guid.NewGuid().ToString(),
                    Idioma = Guid.NewGuid().ToString(),
                    Nombre = Guid.NewGuid().ToString(),
                    Periodicidad = Guid.NewGuid().ToString(),
                    PeriodicidadCodigoOficialExterno = Guid.NewGuid().ToString(),
                    PeriodoLectivo = Guid.NewGuid().ToString(),
                    TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                    {
                        Id = 2,
                        Nombre = Guid.NewGuid().ToString()
                    }
                }
            }).ToList();

            //ACT
            var actual = sut.Object.ConvertToAsignaturasPlanSuperadas(elementosSuperados);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(elementosSuperados.Count, actual.Count);
            foreach (var es in elementosSuperados)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.Asignatura.Codigo == es.Asignatura.Codigo));
                Assert.NotNull(actual.FirstOrDefault(a =>
                    a.Asignatura.CodigoOficialExterno == es.Asignatura.CodigoOficialExterno));
                Assert.NotNull(actual.FirstOrDefault(
                    a => a.Asignatura.IdentificadorOficialExterno == es.Asignatura.IdentificadorOficialExterno));
                Assert.NotNull(actual.FirstOrDefault(a => a.Asignatura.Idioma == es.Asignatura.Idioma));
                Assert.NotNull(actual.FirstOrDefault(a => a.Asignatura.Nombre == es.Asignatura.Nombre));
                Assert.NotNull(actual.FirstOrDefault(a => a.Asignatura.Periodicidad == es.Asignatura.Periodicidad));
                Assert.NotNull(actual.FirstOrDefault(
                    a => a.Asignatura.PeriodicidadCodigoOficialExterno ==
                         es.Asignatura.PeriodicidadCodigoOficialExterno));
                Assert.NotNull(actual.FirstOrDefault(a => a.Asignatura.PeriodoLectivo == es.Asignatura.PeriodoLectivo));
                Assert.NotNull(actual.FirstOrDefault(a =>
                    a.Asignatura.TipoAsignatura.Id == es.Asignatura.TipoAsignatura.Id));
            }

            Assert.Contains(actual, a => string.IsNullOrWhiteSpace(a.Asignatura.Curso));
            sut.Verify(s => s.ConvertToDatosGestor(It.IsAny<AsignaturaPlanErpAcademicoModel>()),
                Times.Exactly(elementosSuperados.Count));
        }

        #endregion

        #region ConvertToDatosGestor

        [Fact(DisplayName = "Cuando se convierte al modelo para los datos de gestor Devuelve ok")]
        public void ConvertToDatosGestor_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturaPlanErpAcademicoModel = new AsignaturaPlanErpAcademicoModel
            {
                Asignatura = new AsignaturaErpAcademicoModel
                {
                    DatosGestor = new DatosGestorErpAcademicoModel
                    {
                        Superado = true,
                        Ects = "5,5",
                        AnyoAcademico = "2022",
                        Convocatoria = "ORD",
                        IdAsignaturaGestor = "1",
                        NotaAlfanumerica = Guid.NewGuid().ToString(),
                        NotaNumerica = Guid.NewGuid().ToString(),
                        Observaciones = Guid.NewGuid().ToString(),
                        ReconocimientosOrigen = Enumerable.Range(1, 3).Select(i =>
                            new ReconocimientosOrigenErpAcademicoModel
                            {
                                Anyo = $"Anio{i}",
                                Convocatoria = $"{Guid.NewGuid()}-{i}",
                                Ects = i,
                                IdAsignaturaGestor = i.ToString(),
                                NivelAprobacion = i.ToString(),
                                NombreAsignaturaExterna = $"NombreAsignaturaExterna{i}",
                                NombreEstudioExterno = $"NombreEstudioExterno{i}",
                                Nota = i,
                                TipoAsignaturaExterna = $"TipoAsignaturaExterna{i}",
                                TipoReconocimiento = "Reconocimiento"
                            }).ToList()
                    }
                }
            };

            //ACT
            var actual = sut.ConvertToDatosGestor(asignaturaPlanErpAcademicoModel);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.Superado, actual.Superado);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.Ects, actual.Ects);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.AnyoAcademico, actual.AnyoAcademico);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.Convocatoria, actual.Convocatoria);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.IdAsignaturaGestor, actual.IdAsignaturaGestor);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.NotaAlfanumerica, actual.NotaAlfanumerica);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.NotaNumerica, actual.NotaNumerica);
            Assert.Equal(asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.Observaciones, actual.Observaciones);
            foreach (var ro in asignaturaPlanErpAcademicoModel.Asignatura.DatosGestor.ReconocimientosOrigen)
            {
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.Anyo == ro.Anyo));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.Convocatoria == ro.Convocatoria));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.IdAsignaturaGestor == ro.IdAsignaturaGestor));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.NivelAprobacion == ro.NivelAprobacion));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.NombreAsignaturaExterna == ro.NombreAsignaturaExterna));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.NombreEstudioExterno == ro.NombreEstudioExterno));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.TipoAsignaturaExterna == ro.TipoAsignaturaExterna));
                Assert.NotNull(actual.ReconocimientosOrigen.FirstOrDefault(a => a.TipoReconocimiento == ro.TipoReconocimiento));
            }
        }

        [Fact(DisplayName = "Cuando no se encuentran los datos del gestor No realiza ninguna acción")]
        public void ConvertToDatosGestor_SinDatos()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturaPlanErpAcademicoModel = new AsignaturaPlanErpAcademicoModel
            {
                Asignatura = new AsignaturaErpAcademicoModel()
            };

            //ACT
            var actual = sut.ConvertToDatosGestor(asignaturaPlanErpAcademicoModel);

            //ASSERT
            Assert.NotNull(actual);
            Assert.False(actual.Superado);
            Assert.Null(actual.Ects);
            Assert.Null(actual.AnyoAcademico);
            Assert.Null(actual.Convocatoria);
            Assert.Null(actual.IdAsignaturaGestor);
            Assert.Null(actual.NotaAlfanumerica);
            Assert.Null(actual.NotaNumerica);
            Assert.Null(actual.Observaciones);
        }

        #endregion

        #region ConvertToArcos

        [Fact(DisplayName = "Cuando se convierte al modelo de datos arcos Devuelve ok")]
        public void ConvertToArcos_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var arcos = Enumerable.Range(1, 3).Select(i => new ArcoErpAcademicoModel
            {
                Id = i,
                BloquesSuperados = new List<int> { 1 },
                Descripcion = $"Descripcion{i}",
                IdNodoDestino = i,
                IdNodoOrigen = i + 1,
                Nombre = Guid.NewGuid().ToString()
            }).ToList();

            //ACT
            var actual = sut.ConvertToArcos(arcos);

            //ASSERT
            Assert.NotNull(actual);
            Assert.NotNull(actual.FirstOrDefault(a => a.BloquesSuperados.Count == 1));
            foreach (var arco in arcos)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.Id == arco.Id));
                Assert.NotNull(actual.FirstOrDefault(a => a.Descripcion == arco.Descripcion));
                Assert.NotNull(actual.FirstOrDefault(a => a.IdNodoDestino == arco.IdNodoDestino));
                Assert.NotNull(actual.FirstOrDefault(a => a.IdNodoOrigen == arco.IdNodoOrigen));
                Assert.NotNull(actual.FirstOrDefault(a => a.Nombre == arco.Nombre));
            }
        }

        #endregion

        #region ConvertToBloquesSuperados

        [Fact(DisplayName = "Cuando se convierte al modelo de datos bloques superados Devuelve ok")]
        public void ConvertToBloquesSuperados_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var asignaturasBloquesEsperadas = Enumerable.Range(1, 2).Select(i =>
                new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                {
                    IdAsignaturaPlan = i
                }).ToList();
            sut.Setup(s => s.ConvertToAsignaturasBloque(It.IsAny<List<AsignaturaPlanBloqueErpAcademicoModel>>()))
                .Returns(asignaturasBloquesEsperadas);

            var subBloquesEsperados = Enumerable.Range(1, 2).Select(i =>
                new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = i
                }).ToList();
            sut.Setup(s => s.ConvertToSubBloques(It.IsAny<List<SubBloqueErpAcademicoModel>>()))
                .Returns(subBloquesEsperados);

            var bloquesSuperado = Enumerable.Range(1, 3).Select(i => new BloqueSuperadoErpAcademicoModel
            {
                Id = i,
                CreditosMinimos = i,
                CreditosObtenidos = i + 1,
                Descripcion = $"Descripcion{i}",
                Nombre = $"Nombre{i}"
            }).ToList();

            //ACT
            var actual = sut.Object.ConvertToBloquesSuperados(bloquesSuperado);

            //ASSERT
            Assert.NotNull(actual);
            foreach (var bloque in bloquesSuperado)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.Id == bloque.Id));
                Assert.NotNull(actual.FirstOrDefault(a => a.Descripcion == bloque.Descripcion));
                Assert.NotNull(actual.FirstOrDefault(a => a.Nombre == bloque.Nombre));
            }

            sut.Verify(s => s.ConvertToAsignaturasBloque(It.IsAny<List<AsignaturaPlanBloqueErpAcademicoModel>>()),
                Times.Exactly(bloquesSuperado.Count));
            sut.Verify(s => s.ConvertToSubBloques(It.IsAny<List<SubBloqueErpAcademicoModel>>()),
                Times.Exactly(bloquesSuperado.Count));
        }

        #endregion

        #region ConvertToAsignaturasBloque

        [Fact(DisplayName = "Cuando se convierte al modelo de datos asignaturas bloques Devuelve ok")]
        public void ConvertToAsignaturasBloque_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturasErp = Enumerable.Range(1, 3).Select(i => new AsignaturaPlanBloqueErpAcademicoModel
            {
                IdAsignaturaPlan = i
            }).ToList();

            //ACT
            var actual = sut.ConvertToAsignaturasBloque(asignaturasErp);

            //ASSERT
            Assert.NotNull(actual);
            foreach (var ap in asignaturasErp)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.IdAsignaturaPlan == ap.IdAsignaturaPlan));
            }
        }

        #endregion

        #region ConvertToSubBloques

        [Fact(DisplayName = "Cuando se convierte al modelo de sub bloques Devuelve ok")]
        public void ConvertToSubBloques_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.ConvertToAsignaturaSubBloque(It.IsAny<List<AsignaturaPlanSubBloqueErpAcademicoModel>>()))
                .Returns(new List<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel>());

            var subBloquesErp = Enumerable.Range(1, 3).Select(i => new SubBloqueErpAcademicoModel
            {
                Id = i,
                CreditosMinimos = i,
                CreditosObtenidos = i + 1,
                Descripcion = $"Descripcion{i}",
                Nombre = $"Nombre{i}",
                TipoSubBloque = new TipoSubBloqueErpAcademicoModel
                {
                    Id = i
                }
            }).ToList();

            //ACT
            var actual = sut.Object.ConvertToSubBloques(subBloquesErp);

            //ASSERT
            Assert.NotNull(actual);
            foreach (var sb in subBloquesErp)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.Id == sb.Id));
                Assert.NotNull(actual.FirstOrDefault(a => a.Descripcion == sb.Descripcion));
                Assert.NotNull(actual.FirstOrDefault(a => a.Nombre == sb.Nombre));
            }

            sut.Verify(s => s.ConvertToAsignaturaSubBloque(It.IsAny<List<AsignaturaPlanSubBloqueErpAcademicoModel>>()),
                Times.Exactly(subBloquesErp.Count));
        }

        #endregion

        #region ConvertToAsignaturaSubBloque

        [Fact(DisplayName = "Cuando se convierte al modelo de datos sub bloques Devuelve ok")]
        public void ConvertToAsignaturaSubBloque_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturasErp = Enumerable.Range(1, 3).Select(i => new AsignaturaPlanSubBloqueErpAcademicoModel
            {
                IdAsignaturaPlan = i
            }).ToList();

            //ACT
            var actual = sut.ConvertToAsignaturaSubBloque(asignaturasErp);

            //ASSERT
            Assert.NotNull(actual);
            foreach (var ap in asignaturasErp)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.IdAsignaturaPlan == ap.IdAsignaturaPlan));
            }
        }

        #endregion

        #region SetAsignaturasReconocidas

        [Fact(DisplayName = "Cuando se setea los datos de asignaturas reconocidas Devuelve ok")]
        public void SetAsignaturasReconocidas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.SetAsignaturaReconocida(It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>()))
                .Returns(new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel());

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = new List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel>()
                    }
                }
            };

            var asignaturas = Enumerable.Range(1, 2).Select(i =>
                new AsignaturaErpAcademicoExpedientesIntegrationModel
                {
                    Superado = i % 2 == 0
                }).ToList();

            //ACT
            sut.Object.SetAsignaturasReconocidas(alumnoPuedeTitularseDto, asignaturas);

            //ASSERT
            sut.Verify(s => s.SetAsignaturaReconocida(It.IsAny<AsignaturaErpAcademicoExpedientesIntegrationModel>()),
                Times.Exactly(asignaturas.Count));
            Assert.Single(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.Single(alumnoPuedeTitularseDto.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
        }

        #endregion

        #region SetAsignaturaReconocida

        [Fact(DisplayName = "Cuando se setean los valores de asignatura reconocida Devuelve ok")]
        public void SetAsignaturaReconocida_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var nombreAsignaturaReconocidaEsperada = Guid.NewGuid().ToString();
            sut.Setup(s => s.SetNombreAsignaturaReconocida(It.IsAny<int>()))
                .Returns(nombreAsignaturaReconocidaEsperada);
            sut.Setup(s =>
                s.SetReconocimientosOrigenAsignaturasDatosGestor(
                    It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>()));

            var asignaturaErp = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                IdAsignatura = 1,
                Ects = 1.56d,
                IdAsignaturaGestor = 1,
                Superado = true,
                NotaNumerica = 1.56d,
                NotaAlfanumerica = Guid.NewGuid().ToString(),
                AnyoAcademico = Guid.NewGuid().ToString(),
                Convocatoria = Guid.NewGuid().ToString(),
                Observaciones = Guid.NewGuid().ToString(),
                ReconocimientosOrigen = Enumerable.Range(1, 2).Select(i => new ReconocimientosOrigenModel
                {
                    IdAsignaturaGestor = i.ToString()
                }).ToList()
            };

            //ACT
            var actual = sut.Object.SetAsignaturaReconocida(asignaturaErp);

            //ASSERT
            sut.Verify(s => s.SetNombreAsignaturaReconocida(It.IsAny<int>()), Times.Once());
            sut.Verify(s =>
                s.SetReconocimientosOrigenAsignaturasDatosGestor(
                    It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>()), Times.Once);
            Assert.Equal(asignaturaErp.IdAsignatura, actual.Id);
            Assert.Equal(asignaturaErp.IdAsignatura, actual.Asignatura.Id);
            Assert.Equal(nombreAsignaturaReconocidaEsperada, actual.Asignatura.Nombre);
            Assert.Equal(asignaturaErp.IdAsignaturaGestor.ToString(), actual.Asignatura.DatosGestor.IdAsignaturaGestor);
            Assert.Equal(asignaturaErp.Superado, actual.Asignatura.DatosGestor.Superado);
            Assert.Equal(asignaturaErp.Ects.ToString(CultureInfo.InvariantCulture), actual.Asignatura.DatosGestor.Ects);
            Assert.Equal(asignaturaErp.NotaNumerica.ToString(CultureInfo.InvariantCulture), actual.Asignatura.DatosGestor.NotaNumerica);
            Assert.Equal(asignaturaErp.NotaAlfanumerica, actual.Asignatura.DatosGestor.NotaAlfanumerica);
            Assert.Equal(asignaturaErp.AnyoAcademico, actual.Asignatura.DatosGestor.AnyoAcademico);
            Assert.Equal(asignaturaErp.Convocatoria, actual.Asignatura.DatosGestor.Convocatoria);
            Assert.Equal(asignaturaErp.Observaciones, actual.Asignatura.DatosGestor.Observaciones);
            Assert.Equal(asignaturaErp.ReconocimientosOrigen.Count, actual.Asignatura.DatosGestor.ReconocimientosOrigen.Count);
        }

        #endregion

        #region SetNombreAsignaturaReconocida

        [Fact(DisplayName = "Cuando se obtiene el nombre de asignatura reconocida de tipo reconocimiento Devuelve ok")]
        public void SetNombreAsignaturaReconocida_TipoReconocimiento()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const string asignaturaReconocidaEsperado = "09990011 - Reconocimiento por Optatividad";

            //ACT
            var actual = sut.SetNombreAsignaturaReconocida(-2);

            //ASSERT
            Assert.Equal(asignaturaReconocidaEsperado, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el nombre de asignatura reconocida de tipo actividades Devuelve ok")]
        public void SetNombreAsignaturaReconocida_TipoActividades()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const string asignaturaReconocidaEsperado = "09990011 - Actividades de Extensión Universitaria";

            //ACT
            var actual = sut.SetNombreAsignaturaReconocida(-3);

            //ASSERT
            Assert.Equal(asignaturaReconocidaEsperado, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el nombre de asignatura reconocida de tipo actividades Devuelve ok")]
        public void SetNombreAsignaturaReconocida_TipoSeminario()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const string asignaturaReconocidaEsperado = "09990011 - Seminario";

            //ACT
            var actual = sut.SetNombreAsignaturaReconocida(-1);

            //ASSERT
            Assert.Equal(asignaturaReconocidaEsperado, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el nombre de asignatura vacío Devuelve ok")]
        public void SetNombreAsignaturaReconocida_SinTipo()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.SetNombreAsignaturaReconocida(1000);

            //ASSERT
            Assert.Empty(actual);
        }

        #endregion

        #region SetReconocimientosOrigenAsignaturasDatosGestor

        [Fact(DisplayName = "Cuando id asignatura es menor que 1 e id asignatura gestor es mayor que 0 Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoSeminario1()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-1",
                IdAsignaturaGestor = "1",
            };

            const string errorEsperado = "Seminario indicado incorrectamente.";

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            Assert.Equal(errorEsperado, datosGestor.ErroresReconocimientosOrigen.First());
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        [Trait("Category", "Cuando id asignatura es mayor que 0 e id asignaturaGestor es mayor que 0 Asigna Error")]
        [Fact]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_TipoReconocimientoSeminario()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            const string tipoReconocimiento = "Seminario";
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "1",
                IdAsignaturaGestor = "1",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                    {
                        TipoReconocimiento = tipoReconocimiento
                    }
                }
            };

            const string errorEsperado = "Seminario indicado incorrectamente.";

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
            Assert.Equal(errorEsperado, datosGestor.ErroresReconocimientosOrigen.First());
        }

        [Theory(DisplayName = "Cuando idAsignatura es igual a menos 4 e idAsignaturaGestor es mayor que 0 o idAsignatura es mayor que 0 e idAsignaturaGestor es igual a menos 4 Asigna Error")]
        [InlineData("-4", "1")]
        [InlineData("1", "-4")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoSeminario4(string idAsignatura, string idAsignaturaGestor)
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = idAsignatura,
                IdAsignaturaGestor = idAsignaturaGestor
            };

            const string errorEsperado = "Seminario indicado incorrectamente.";

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            Assert.Equal(datosGestor.ErroresReconocimientosOrigen[0], errorEsperado);
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando idAsignatura es igual que menos 2 e idAsignaturaGestor es distinto que menos 999999 Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoSeminario4_2()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-2",
                IdAsignaturaGestor = "1"
            };

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        [Theory(DisplayName = "Cuando idAsignatura es igual a menos 3 e idAsignaturaGestor es mayor que 0 o idAsignatura es mayor que 0 e idAsignaturaGestor es igual a menos 3 Asigna Error")]
        [InlineData("-3", "1")]
        [InlineData("1", "-3")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_OptativaIncorrecta(string idAsignatura, string idAsignaturaGestor)
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            const string tipoReconocimiento = "Reconocimiento";
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = idAsignatura,
                IdAsignaturaGestor = idAsignaturaGestor,
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                    {
                        TipoReconocimiento = tipoReconocimiento,
                        NombreAsignaturaExterna = tipoReconocimiento
                    }
                }
            };

            const string errorEsperado = "Optatividad indicada incorrectamente.";

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
            Assert.Equal(datosGestor.ErroresReconocimientosOrigen[0], errorEsperado);
        }

        [Fact(DisplayName = "Cuando idAsignatura es igual que menos 4 e idAsignaturaGestor es igual que menos 4 No Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoSeminario9()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-4",
                IdAsignaturaGestor = "-4",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                }
            };

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando idAsignatura es igual que menos 2 e idAsignaturaGestor es igual que 999999 No Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_ReconocimientoIncorrecto()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-2",
                IdAsignaturaGestor = "999999",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                }
            };

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando idAsignatura es igual que menos 2 e idAsignaturaGestor es igual que 999999 No Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoActividades_2()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-3",
                IdAsignaturaGestor = "999999",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                }
            };

            const string errorEsperado = "Optatividad indicada incorrectamente.";

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
            Assert.Equal(datosGestor.ErroresReconocimientosOrigen[0], errorEsperado);
        }

        [Fact(DisplayName = "Cuando idAsignatura es igual que menos 3 e idAsignaturaGestor es igual que menos 3 No Asigna Error")]
        public void SetReconocimientosOrigenAsignaturasDatosGestor_AsignaturaReconocidaTipoActividades()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()));

            const string nombreAsignaturaExterna = "Nombre";
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-3",
                IdAsignaturaGestor = "-3",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                    {
                        NombreAsignaturaExterna = nombreAsignaturaExterna
                    }
                }
            };

            //ACT
            sut.Object.SetReconocimientosOrigenAsignaturasDatosGestor(datosGestor);

            //ASSERT
            Assert.NotNull(datosGestor.ReconocimientosOrigen);
            Assert.Empty(datosGestor.ErroresReconocimientosOrigen);
            sut.Verify(s =>
                s.SetDatosTipoReconocimientoOrigen(It.IsAny<DatosGestorAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region SetDatosTipoReconocimientoOrigen

        [Fact(DisplayName = "Cuando hay datos y tipo reconocimiento origen es null Asigna datos")]
        public void SetDatosTipoReconocimientoOrigen_TipoReconocimientoNull()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const string tipoReconocimiento = "Seminario";
            const double ects = 0;
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-1",
                IdAsignaturaGestor = "1"
            };

            //ACT
            sut.SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimiento);

            //ASSERT
            Assert.NotNull(datosGestor.ReconocimientosOrigen);
            Assert.Equal(datosGestor.ReconocimientosOrigen[0].TipoReconocimiento, tipoReconocimiento);
            Assert.Equal(datosGestor.ReconocimientosOrigen[0].Ects, ects);
        }

        [Fact(DisplayName = "Cuando hay datos y tipo reconocimiento seminario no es null asigna datos")]
        public void SetDatosTipoReconocimientoOrigen_Ok_TipoReconocimientoSeminario()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const string tipoReconocimiento = "Seminario";
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-1",
                IdAsignaturaGestor = "1",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                }
            };

            //ACT
            sut.SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimiento);

            //ASSERT
            Assert.NotNull(datosGestor.ReconocimientosOrigen);
            Assert.Equal(datosGestor.ReconocimientosOrigen[0].TipoReconocimiento, tipoReconocimiento);
        }

        [Fact(DisplayName = "Cuando hay datos y tipo reconocimiento  no es null asigna datos")]
        public void SetDatosTipoReconocimientoOrigen_Ok_TipoReconocimiento()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var tipoAsignaturaEsperado = Guid.NewGuid().ToString();
            var datosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = "-1",
                IdAsignaturaGestor = "1",
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                    {
                        NombreAsignaturaExterna = tipoAsignaturaEsperado
                    }
                }
            };

            //ACT
            sut.SetDatosTipoReconocimientoOrigen(datosGestor, "Reconocimiento");

            //ASSERT
            Assert.NotNull(datosGestor.ReconocimientosOrigen);
            Assert.Equal(datosGestor.ReconocimientosOrigen[0].TipoReconocimiento, tipoAsignaturaEsperado);
        }

        #endregion

        #region SetElementosNoSuperados

        [Fact(DisplayName = "Cuando hay asignaturas no superadas se guardan dentro de elementos no superados")]
        public void SetElementosNoSuperados_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var bloquesNoSuperados = Enumerable.Range(1, 3)
                .Select(i => new BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = i
                }).ToList();
            sut.Setup(s =>
                s.SetBloquesAndSubBloquesNoSuperados(
                    It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int[]>())).Returns(bloquesNoSuperados);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, 3).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i
                            }).ToList()
                    }
                }
            };

            var idsAsignaturasNoSuperadas = new[] { 1, 2 };

            //ACT
            sut.Object.SetElementosNoSuperados(idsAsignaturasNoSuperadas,
                new List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>(), alumnoPuedeTitularseDto);

            //ASSERT
            Assert.NotEmpty(alumnoPuedeTitularseDto.ElementosNoSuperados.BloquesNoSuperados);
            Assert.Equal(bloquesNoSuperados.Count,
                alumnoPuedeTitularseDto.ElementosNoSuperados.BloquesNoSuperados.Count);
            Assert.NotEmpty(alumnoPuedeTitularseDto.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
            Assert.Equal(idsAsignaturasNoSuperadas.Length,
                alumnoPuedeTitularseDto.ElementosNoSuperados.AsignaturasPlanNoSuperadas.Count);
            sut.Verify(s =>
                s.SetBloquesAndSubBloquesNoSuperados(
                    It.IsAny<List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int[]>()), Times.Once);
        }

        #endregion

        #region SetBloquesAndSubBloquesNoSuperados

        [Fact(DisplayName = "Cuando no tiene asignaturas superadas Retona listado con bloques no superados vacío")]
        public void SetBloquesAndSubBloquesNoSuperados_SinAsignaturasSuperadas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.ProcesarAsignaturaNoSuperadas(It.IsAny<int>(), It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>(),
                It.IsAny<List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()));
            sut.Setup(s => s.ProcesarAsignaturaBloqueNoSuperada(It.IsAny<AlumnoPuedeTitularseDto>(),
                It.IsAny<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>(),
                It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()));

            var idsAsignaturasNoSuperadas = new[] { 1, 2 };

            var asignaturasBloqueNoSuperadas = Enumerable.Range(1, 3).Select(i =>
                new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                {
                    IdAsignaturaPlan = i
                }).ToList();

            //ACT
            var actual = sut.Object.SetBloquesAndSubBloquesNoSuperados(asignaturasBloqueNoSuperadas,
                new AlumnoPuedeTitularseDto(),
                idsAsignaturasNoSuperadas);

            //ASSERT
            sut.Verify(s => s.ProcesarAsignaturaNoSuperadas(It.IsAny<int>(), It.IsAny<AlumnoPuedeTitularseDto>(),
                    It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()),
                Times.Exactly(idsAsignaturasNoSuperadas.Length));
            sut.Verify(s => s.ProcesarAsignaturaBloqueNoSuperada(It.IsAny<AlumnoPuedeTitularseDto>(),
                    It.IsAny<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()),
                Times.Exactly(asignaturasBloqueNoSuperadas.Count));
            Assert.NotNull(actual);
        }

        #endregion

        #region ProcesarAsignaturaNoSuperadas

        [Fact(DisplayName = "Cuando se procesan las asignaturas no superadas y no tiene sub bloques No realiza ninguna acción")]
        public void ProcesarAsignaturaNoSuperadas_SinSubBloques()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 3).Select(_ =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel()).ToList()
                    }
                }
            };

            var result = new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>
            {
                new()
            };

            //ACT
            sut.ProcesarAsignaturaNoSuperadas(1,
                alumnoPuedeTitularseDto,
                new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>(),
                new List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>());

            //ASSERT
            Assert.Empty(result.First().AsignaturasBloqueNoSuperadas);
            Assert.Empty(result.First().SubBloquesNoSuperados);
        }

        [Fact(DisplayName = "Cuando se procesan las asignaturas no superadas Devuelve ok")]
        public void ProcesarAsignaturaNoSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.SetSubBloquesNoSuperados(It.IsAny<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel>(),
                It.IsAny<int>(), It.IsAny<List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>(),
                It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()));

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 3).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                SubBloquesSuperados = Enumerable.Range(1, 2).Select(j =>
                                    new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        Id = j,
                                        AsignaturasSubBloqueSuperadas = Enumerable.Range(1, 2).Select(k =>
                                            new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                            {
                                                IdAsignaturaPlan = k
                                            }).ToList()
                                    }).ToList()
                            }).ToList()
                    }
                }
            };

            var result = new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>
            {
                new()
                {
                    Id = 1
                }
            };

            var subBloquesNoSuperados = new List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>
            {
                new()
                {
                    AsignaturasSubBloqueNoSuperadas = new List<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel>
                    {
                        new()
                        {
                            IdAsignaturaPlan = 1
                        }
                    }
                }
            };

            //ACT
            sut.Object.ProcesarAsignaturaNoSuperadas(1, alumnoPuedeTitularseDto, result, subBloquesNoSuperados);

            //ASSERT
            sut.Verify(s => s.SetSubBloquesNoSuperados(It.IsAny<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel>(),
                    It.IsAny<int>(), It.IsAny<List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>(),
                    It.IsAny<List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>>()),
                Times.Exactly(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Count -
                              result.Count));
            Assert.Equal(1, subBloquesNoSuperados.Last().AsignaturasSubBloqueNoSuperadas.First().IdAsignaturaPlan);
        }

        #endregion

        #region SetSubBloquesNoSuperados

        [Fact(DisplayName = "Cuando no tiene asignaturas superadas Retona listado con bloques no superados vacío")]
        public void SetSubBloquesNoSuperados_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var bloque = new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel()
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Descripcion = Guid.NewGuid().ToString(),
                CreditosMinimos = 3d,
                SubBloquesSuperados = Enumerable.Range(1, 2)
                    .Select(i => new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = i,
                        Nombre = Guid.NewGuid().ToString(),
                        Descripcion = Guid.NewGuid().ToString(),
                        CreditosMinimos = 2d,
                        AsignaturasSubBloqueSuperadas = i % 2 == 0
                            ? Enumerable.Range(1, 2).Select(j =>
                                new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                {
                                    IdAsignaturaPlan = j
                                }).ToList()
                            : Array.Empty<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel>().ToList()
                    }).ToList()
            };

            var subBloqueNoSuperado = new List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();
            var result = new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();

            var subBloqueSuperado = bloque.SubBloquesSuperados.First(ss =>
                ss.AsignaturasSubBloqueSuperadas.Any(asb => asb.IdAsignaturaPlan == 1));

            //ACT
            sut.SetSubBloquesNoSuperados(bloque, 1, subBloqueNoSuperado, result);

            //ASSERT
            var subBloqueEsperado = subBloqueNoSuperado.First();
            Assert.Single(subBloqueNoSuperado);
            Assert.Equal(subBloqueEsperado.Nombre, subBloqueSuperado.Nombre);
            Assert.Equal(subBloqueEsperado.Descripcion, subBloqueSuperado.Descripcion);
            Assert.Equal(subBloqueEsperado.CreditosMinimos, subBloqueSuperado.CreditosMinimos);
            Assert.Equal(0, subBloqueEsperado.CreditosObtenidos);
            Assert.Equal(bloque.Descripcion, result.First().Descripcion);
            Assert.Equal(bloque.Nombre, result.First().Nombre);
            Assert.Equal(bloque.CreditosMinimos, result.First().CreditosMinimos);
            Assert.Equal(0, result.First().CreditosObtenidos);
        }

        #endregion

        #region ProcesarAsignaturaBloqueNoSuperada

        [Fact(DisplayName = "Cuando se procesan las asignaturas bloques no superadas y existe el bloque Devuelve ok")]
        public void ProcesarAsignaturaBloqueNoSuperada_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i + 10,
                                Nombre = Guid.NewGuid().ToString(),
                                Descripcion = Guid.NewGuid().ToString(),
                                AsignaturasBloqueSuperadas = Enumerable.Range(1, 3).Select(j =>
                                    new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        IdAsignaturaPlan = j
                                    }).ToList()
                            }).ToList()
                    }
                }
            };

            var asignaturaPlanBloque = new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignaturaPlan = 1
            };

            var result = Enumerable.Range(1, 2).Select(i => new BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
            {
                Id = i
            }).ToList();

            const int cantidadBloquesEsperado = 3;

            //ACT
            sut.ProcesarAsignaturaBloqueNoSuperada(alumnoPuedeTitularseDto, asignaturaPlanBloque, result);

            //ASSERT
            Assert.Equal(cantidadBloquesEsperado, result.Count);
        }

        [Fact(DisplayName = "Cuando se procesan las asignaturas bloques no superadas y no existe el bloque No realiza ninguna acción")]
        public void ProcesarAsignaturaBloqueNoSuperada_SinBloque_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                Nombre = Guid.NewGuid().ToString(),
                                Descripcion = Guid.NewGuid().ToString(),
                                AsignaturasBloqueSuperadas = Enumerable.Range(1, 3).Select(j =>
                                    new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        IdAsignaturaPlan = j
                                    }).ToList()
                            }).ToList()
                    }
                }
            };

            var asignaturaPlanBloque = new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignaturaPlan = 1
            };

            const int cantidadNoAlteradaEsperada = 2;
            var result = Enumerable.Range(1, cantidadNoAlteradaEsperada).Select(i =>
                new BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = i
                }).ToList();

            //ACT
            sut.ProcesarAsignaturaBloqueNoSuperada(alumnoPuedeTitularseDto, asignaturaPlanBloque, result);

            //ASSERT
            Assert.Equal(cantidadNoAlteradaEsperada, result.Count);
        }

        #endregion

        #region RemoverAsignaturasNoSuperadas

        [Fact(DisplayName = "Cuando se elimina las asignaturas no superadas Devuelve ok")]
        public void RemoverAsignaturasNoSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var idsAsignaturasNoSuperadas = Enumerable.Range(1, 3).Select(i => i).ToArray();

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, idsAsignaturasNoSuperadas.Length).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i
                            }).ToList()
                    }
                }
            };

            //ACT
            sut.RemoverAsignaturasNoSuperadas(idsAsignaturasNoSuperadas, alumnoPuedeTitularseDto);

            //ASSERT
            Assert.Empty(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
        }

        #endregion

        #region RemoverBloquesAsignaturasNoSuperadas

        [Fact(DisplayName = "Cuando se elimina los bloques asignaturas no superadas Devuelve ok")]
        public void RemoverBloquesAsignaturasNoSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturasPlanBloque = Enumerable.Range(1, 3).Select(i =>
                new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                {
                    IdAsignaturaPlan = i
                }).ToList();

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                AsignaturasBloqueSuperadas = Enumerable.Range(1, 2).Select(j =>
                                    new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        IdAsignaturaPlan = j
                                    }).ToList()
                            }).ToList()
                    }
                }
            };

            //ACT
            sut.RemoverBloquesAsignaturasNoSuperadas(asignaturasPlanBloque, alumnoPuedeTitularseDto);

            //ASSERT
            Assert.Equal(asignaturasPlanBloque.Count - 1,
                alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Count);
        }

        #endregion

        #region RemoverSubBloquesAsignaturasNoSuperadas

        [Fact(DisplayName = "Cuando se elimina los sub bloques asignaturas no superadas Devuelve ok")]
        public void RemoverSubBloquesAsignaturasNoSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturasPlanSubBloque = Enumerable.Range(1, 3).Select(i => i).ToArray();

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                SubBloquesSuperados = Enumerable.Range(1, 2).Select(j =>
                                    new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        Id = j,
                                        AsignaturasSubBloqueSuperadas = Enumerable.Range(1, 2).Select(k =>
                                            new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                            {
                                                IdAsignaturaPlan = k
                                            }).ToList()
                                    }).ToList()
                            }).ToList()
                    }
                }
            };

            //ACT
            sut.RemoverSubBloquesAsignaturasNoSuperadas(asignaturasPlanSubBloque, alumnoPuedeTitularseDto);

            //ASSERT
            Assert.Equal(asignaturasPlanSubBloque.Length - 1,
                alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Count);
        }

        #endregion

        #region SetAsignaturasConIntegracionErpAcademico

        [Fact(DisplayName = "Cuando se setean las asignaturas plan con las de erp Devuelve ok")]
        public void SetAsignaturasConIntegracionErpAcademico_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            { CallBase = true};

            var asignaturaPlanPuedeTitularse = new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
            {
                Id = 1,
                Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                {
                    DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel()
                }
            };

            var datosAcademicos = Enumerable.Range(1, 2).Select(i => new AsignaturaPlanTitulacionErpAcademico
            {
                IdAsignaturaPlan = i.ToString(),
                Nombre = Guid.NewGuid().ToString(),
                Codigo = Guid.NewGuid().ToString(),
                Creditos = 1d,
                CodigoOficialExterno = Guid.NewGuid().ToString(),
                IdentificadorOficialExterno = Guid.NewGuid().ToString(),
                Periodicidad = Guid.NewGuid().ToString(),
                PeriodicidadCodigoOficialExterno = Guid.NewGuid().ToString(),
                PeriodoLectivo = Guid.NewGuid().ToString(),
                Idioma = Guid.NewGuid().ToString(),
                TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                {
                    Id = i,
                    Nombre = Guid.NewGuid().ToString(),
                    Orden = i,
                    Simbolo = Guid.NewGuid().ToString(),
                    TrabajoFinEstudio = i % 2 == 0
                }
            }).ToList();

            var datosGestor = Enumerable.Range(1, 2).Select(i => new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                IdAsignatura = i,
                IdAsignaturaGestor = i + 1,
                Ects = 1d,
                NotaNumerica = 2d,
                NotaAlfanumerica = Guid.NewGuid().ToString(),
                Superado = i % 2 == 0,
                AnyoAcademico = Guid.NewGuid().ToString(),
                Convocatoria = Guid.NewGuid().ToString(),
                Observaciones = Guid.NewGuid().ToString(),
                ReconocimientosOrigen = new List<ReconocimientosOrigenModel>()
            }).ToList();

            var asignaturaErpEsperada =
                datosAcademicos.First(da => da.IdAsignaturaPlan == asignaturaPlanPuedeTitularse.Id.ToString());
            var datoGestorEsperado = datosGestor.First(dg => dg.IdAsignatura == asignaturaPlanPuedeTitularse.Id);

            //ACT
            sut.Object.SetAsignaturasConIntegracionErpAcademico(asignaturaPlanPuedeTitularse, datosAcademicos, datosGestor);

            //ASSERT
            Assert.Equal(asignaturaPlanPuedeTitularse.Id.ToString(), asignaturaErpEsperada.IdAsignaturaPlan);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.Nombre, asignaturaErpEsperada.Nombre);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.Codigo, asignaturaErpEsperada.Codigo);
            Assert.Null(asignaturaPlanPuedeTitularse.Asignatura.Curso);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.CodigoOficialExterno,
                asignaturaErpEsperada.CodigoOficialExterno);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.IdentificadorOficialExterno,
                asignaturaErpEsperada.IdentificadorOficialExterno);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.Periodicidad, asignaturaErpEsperada.Periodicidad);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.PeriodicidadCodigoOficialExterno,
                asignaturaErpEsperada.PeriodicidadCodigoOficialExterno);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.PeriodoLectivo, asignaturaErpEsperada.PeriodoLectivo);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.Idioma, asignaturaErpEsperada.Idioma);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.TipoAsignatura.Id,
                asignaturaErpEsperada.TipoAsignatura.Id);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.TipoAsignatura.Nombre,
                asignaturaErpEsperada.TipoAsignatura.Nombre);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.TipoAsignatura.Orden,
                asignaturaErpEsperada.TipoAsignatura.Orden);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.TipoAsignatura.Simbolo,
                asignaturaErpEsperada.TipoAsignatura.Simbolo);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.TipoAsignatura.TrabajoFinEstudio,
                asignaturaErpEsperada.TipoAsignatura.TrabajoFinEstudio);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.IdAsignatura,
                datoGestorEsperado.IdAsignatura.ToString());
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.IdAsignaturaGestor,
                datoGestorEsperado.IdAsignaturaGestor.ToString());
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.NotaAlfanumerica,
                datoGestorEsperado.NotaAlfanumerica);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.Superado,
                datoGestorEsperado.Superado);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.AnyoAcademico,
                datoGestorEsperado.AnyoAcademico);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.Convocatoria,
                datoGestorEsperado.Convocatoria);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.Observaciones,
                datoGestorEsperado.Observaciones);
            Assert.Equal(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.ReconocimientosOrigen.Count,
                datoGestorEsperado.ReconocimientosOrigen.Count);
        }

        [Fact(DisplayName = "Cuando no se obtienen los datos académicos No realiza ninguna acción")]
        public void SetAsignaturasConIntegracionErpAcademico_SinDatosAcademicos()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var asignaturaPlanPuedeTitularse = new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
            {
                Id = 1,
                Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                {
                    DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel()
                }
            };

            var datosAcademicos = Enumerable.Range(2, 4).Select(i => new AsignaturaPlanTitulacionErpAcademico
            {
                IdAsignaturaPlan = i.ToString()
            }).ToList();

            //ACT
            sut.Object.SetAsignaturasConIntegracionErpAcademico(asignaturaPlanPuedeTitularse, datosAcademicos,
                new List<AsignaturaErpAcademicoExpedientesIntegrationModel>());

            //ASSERT
            Assert.Null(asignaturaPlanPuedeTitularse.Asignatura.Nombre);
            Assert.Null(asignaturaPlanPuedeTitularse.Asignatura.DatosGestor.IdAsignatura);
        }

        #endregion

        #region SetExpedicion

        [Fact(DisplayName = "Cuando se setean los datos de expedición Devuelve ok")]
        public async Task SetExpedicion_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient
                .Setup(s => s.GetAsignaturasEspecializacionPlan(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<int>());

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturaPlanPuedeTitularse = new AlumnoPuedeTitularseDto
            {
                Expedicion = new ExpedicionDto()
            };

            var datosExpedicion = new ExpedienteExpedientesIntegrationModel
            {
                ViaAcceso = Guid.NewGuid().ToString(),
                FechaFinEstudio = DateTime.Now,
                FechaExpedicion = DateTime.Now,
                TituloTfmTfg = Guid.NewGuid().ToString(),
                FechaTfmTfg = DateTime.Now,
                NotaMedia = 2d,
                ViasAcceso = new ViasAccesoIntegrationModel
                {
                    Generica = Guid.NewGuid().ToString(),
                    Especifica = Guid.NewGuid().ToString(),
                    GenericaIngles = Guid.NewGuid().ToString(),
                    EspecificaIngles = Guid.NewGuid().ToString()
                },
                IdiomaAcreditacion = new IdiomaAcreditacionIntegrationModel
                {
                    Idioma = Guid.NewGuid().ToString(),
                    Acreditacion = Guid.NewGuid().ToString(),
                    FechaAcreditacion = DateTime.Now
                },
                ItinerariosFinalizados = Enumerable.Range(1, 2).Select(i => new ItinerariosFinalizadosIntegrationModel
                {
                    IdEspecializacionErp = "1",
                    TipoItinerario = new TipoItinerarioIntegrationModel
                    {
                        Id = i,
                        Nombre = Guid.NewGuid().ToString()
                    },
                    Nombre = Guid.NewGuid().ToString(),
                    FechaFin = DateTime.Now
                }).ToList(),
                Practica = new PracticaIntegrationModel
                {
                    CentroPractica = Guid.NewGuid().ToString(),
                    FechaInicio = DateTime.Now,
                    FechaFin = DateTime.Now
                }
            };

            //ACT
            await sut.SetExpedicion(asignaturaPlanPuedeTitularse, datosExpedicion, 1);

            //ASSERT
            mockIErpAcademicoServiceClient
                .Verify(s => s.GetAsignaturasEspecializacionPlan(It.IsAny<int>(), It.IsAny<int>()),
                    Times.Exactly(datosExpedicion.ItinerariosFinalizados.Count));
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ViaAcceso, datosExpedicion.ViaAcceso);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.FechaFinEstudio, datosExpedicion.FechaFinEstudio);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.FechaExpedicion, datosExpedicion.FechaExpedicion);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.TituloTfmTfg, datosExpedicion.TituloTfmTfg);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.FechaTfmTfg, datosExpedicion.FechaTfmTfg);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.NotaMedia, datosExpedicion.NotaMedia);
            Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ViaAcceso);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ViasAcceso.Generica,
                datosExpedicion.ViasAcceso.Generica);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ViasAcceso.Especifica,
                datosExpedicion.ViasAcceso.Especifica);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ViasAcceso.GenericaIngles,
                datosExpedicion.ViasAcceso.GenericaIngles);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ViasAcceso.EspecificaIngles,
                datosExpedicion.ViasAcceso.EspecificaIngles);
            Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.IdiomaAcreditacion);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.IdiomaAcreditacion.Idioma,
                datosExpedicion.IdiomaAcreditacion.Idioma);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.IdiomaAcreditacion.Acreditacion,
                datosExpedicion.IdiomaAcreditacion.Acreditacion);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.IdiomaAcreditacion.FechaAcreditacion,
                datosExpedicion.IdiomaAcreditacion.FechaAcreditacion);
            Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados);
            Assert.NotEmpty(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.Count,
                datosExpedicion.ItinerariosFinalizados.Count);
            foreach (var datoExpedicion in datosExpedicion.ItinerariosFinalizados)
            {
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.TipoItinerario.Id == datoExpedicion.TipoItinerario.Id.ToString()));
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.TipoItinerario.Nombre == datoExpedicion.TipoItinerario.Nombre));
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.Nombre == datoExpedicion.Nombre));
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.FechaFin == datoExpedicion.FechaFin));
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.IdEspecializacionErp == datoExpedicion.IdEspecializacionErp));
                Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.ItinerariosFinalizados.FirstOrDefault(ifa =>
                    ifa.IdEspecializacionErp == datoExpedicion.IdEspecializacionErp));
            }

            Assert.NotNull(asignaturaPlanPuedeTitularse.Expedicion.Practica);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.Practica.CentroPractica,
                datosExpedicion.Practica.CentroPractica);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.Practica.FechaInicio,
                datosExpedicion.Practica.FechaInicio);
            Assert.Equal(asignaturaPlanPuedeTitularse.Expedicion.Practica.FechaFin, datosExpedicion.Practica.FechaFin);
        }

        #endregion

        #region SetCreditosObtenidosAsignaturasSuperadas

        [Fact(DisplayName = "Cuando se setean los créditos de los bloques superados Devuelve ok")]
        public void SetCreditosObtenidosAsignaturasSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetCreditosObtenidosByAsignatura(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int>()))
                .Returns(1d);

            var asignaturaPlanPuedeTitularse = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, 2).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i
                            }).ToList(),
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                AsignaturasBloqueSuperadas = Enumerable.Range(1, 2).Select(j =>
                                    new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        IdAsignaturaPlan = j
                                    }).ToList(),
                                SubBloquesSuperados = Enumerable.Range(1, 2).Select(j =>
                                    new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        Id = j
                                    }).ToList()
                            }).ToList()
                    }
                }
            };


            //ACT
            sut.Object.SetCreditosObtenidosAsignaturasSuperadas(asignaturaPlanPuedeTitularse);

            //ASSERT
            sut.Verify(s => s.GetCreditosObtenidosByAsignatura(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int>()),
                Times.Exactly(4));
            Assert.Contains(asignaturaPlanPuedeTitularse.EsPlanSuperado.ElementosSuperados.BloquesSuperados,
                bs => bs.CreditosObtenidos > 0);
        }

        [Fact(DisplayName = "Cuando se setean los créditos de los sub bloques superados Devuelve ok")]
        public void SetCreditosObtenidosAsignaturasSuperadas_SubBloques_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetCreditosObtenidosByAsignatura(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int>()))
                .Returns(1d);

            var asignaturaPlanPuedeTitularse = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, 2).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i
                            }).ToList(),
                        BloquesSuperados = Enumerable.Range(1, 2).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                SubBloquesSuperados = Enumerable.Range(1, 2).Select(j =>
                                    new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                    {
                                        Id = j,
                                        AsignaturasSubBloqueSuperadas = Enumerable.Range(1, 2).Select(k =>
                                            new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                                            {
                                                IdAsignaturaPlan = k
                                            }).ToList()
                                    }).ToList()
                            }).ToList()
                    }
                }
            };


            //ACT
            sut.Object.SetCreditosObtenidosAsignaturasSuperadas(asignaturaPlanPuedeTitularse);

            //ASSERT
            sut.Verify(s => s.GetCreditosObtenidosByAsignatura(It.IsAny<AlumnoPuedeTitularseDto>(), It.IsAny<int>()),
                Times.Exactly(8));
            Assert.Contains(asignaturaPlanPuedeTitularse.EsPlanSuperado.ElementosSuperados.BloquesSuperados,
                bs => bs.CreditosObtenidos > 0);
            Assert.Contains(
                asignaturaPlanPuedeTitularse.EsPlanSuperado.ElementosSuperados.BloquesSuperados.SelectMany(b =>
                    b.SubBloquesSuperados),
                bs => bs.CreditosObtenidos > 0);
        }

        #endregion

        #region GetCreditosObtenidosByAsignatura

        [Fact(DisplayName = "Cuando se obtienen los créditos por asignatura Devuelve ok")]
        public void GetCreditosObtenidosByAsignatura_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = Enumerable.Range(1, 2).Select(i =>
                            new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                                {
                                    Creditos = 50d
                                }
                            }).ToList()
                    }
                }
            };

            var asignaturaPlanEsperada =
                alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.First();

            //ACT
            var actual = sut.GetCreditosObtenidosByAsignatura(alumnoPuedeTitularseDto, 1);

            //ASSERT
            Assert.True(actual > 0);
            Assert.Equal(asignaturaPlanEsperada.Asignatura.Creditos, actual);
        }

        #endregion

        #region SetArcosSuperados

        [Fact(DisplayName = "Cuando se setean los valores de los arcos superados Devuelve ok")]
        public void SetArcosSuperados_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Arcos = Enumerable.Range(1, 2).Select(i =>
                            new ArcoAlumnoPuedeTitulularseErpAcademicoModel
                            {
                                Id = i,
                                Superado = i % 2 == 0,
                                BloquesSuperados = new List<int> {1, 2, 3}
                            }).ToList(),
                        BloquesSuperados = Enumerable.Range(1, 3).Select(i =>
                            new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                            {
                                Id = i,
                                CreditosMinimos = i % 2 == 0 ? 5 : 4,
                                CreditosObtenidos = i % 2 == 0 ? 4 : 5
                            }).ToList()
                    }
                }
            };

            //ACT
            sut.SetArcosSuperados(alumnoPuedeTitularseDto);

            //ASSERT
            Assert.Contains(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.Arcos,
                asu => asu.Superado);
            Assert.Single(alumnoPuedeTitularseDto.EsPlanSuperado.ElementosSuperados.Arcos,
                asu => !asu.Superado);
        }

        #endregion

        #region SetBloqueos

        [Fact(DisplayName = "Cuando se setean los valores de los bloqueos Devuelve ok")]
        public void SetBloqueos_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.SetAccionesBloqueadas(It.IsAny<List<AccionesBloqueadasIntegrationModel>>()))
                .Returns(new List<AccionBloqueoDto>());

            var alumnoPuedeTitularseDto = new AlumnoPuedeTitularseDto();

            var bloqueos = Enumerable.Range(1, 3).Select(i => new BloqueosIntegrationModel
            {

                Nombre = $"{Guid.NewGuid()}-{i}",
                Observacion = Guid.NewGuid().ToString()
            }).ToList();

            var bloqueoEsperado = bloqueos.Last();

            //ACT
            sut.Object.SetBloqueos(alumnoPuedeTitularseDto, bloqueos);

            //ASSERT
            sut.Verify(s => s.SetAccionesBloqueadas(It.IsAny<List<AccionesBloqueadasIntegrationModel>>()),
                Times.Exactly(bloqueos.Count));
            Assert.NotNull(alumnoPuedeTitularseDto.Bloqueos);
            Assert.Equal(alumnoPuedeTitularseDto.Bloqueos.Nombre, bloqueoEsperado.Nombre);
            Assert.Equal(alumnoPuedeTitularseDto.Bloqueos.Observacion, bloqueoEsperado.Observacion);
            Assert.True(alumnoPuedeTitularseDto.Bloqueos.Bloqueado);
        }

        #endregion

        #region SetAccionesBloqueadas

        [Fact(DisplayName = "Cuando se setean los valores de las acciones bloqueadas Devuelve ok")]
        public void SetAccionesBloqueadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var accionesBloqueadas = Enumerable.Range(1, 3).Select(i => new AccionesBloqueadasIntegrationModel
            {
                CodigoAccion = $"{Guid.NewGuid()}-{i}"
            }).ToList();

            //ACT
            var actual = sut.SetAccionesBloqueadas(accionesBloqueadas);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(accionesBloqueadas.Count, actual.Count);
            foreach (var accion in accionesBloqueadas)
            {
                Assert.NotNull(actual.FirstOrDefault(a => a.CodigoAccion == accion.CodigoAccion));
            }
        }

        #endregion

        #region GetCausasFallosComprobacionMatriculasDocumentacionErp

        [Fact(DisplayName = "Cuando se obtiene las causas fallos matriculas Devuelve ok")]
        public async Task GetCausasFallosComprobacionMatriculasDocumentacionErp_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var alumnoMatriculacionEsperado = new ValidateAlumnoMatriculacionErpAcademicoModel
            {
                MatriculasOk = true
            };
            mockIErpAcademicoServiceClient.Setup(s =>
                    s.ValidateAlumnoMatriculacion(It.IsAny<ValidateAlumnoMatriculacionParameters>()))
                .ReturnsAsync(alumnoMatriculacionEsperado);

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = await sut.GetCausasFallosComprobacionMatriculasDocumentacionErp("12", "13");

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(alumnoMatriculacionEsperado.MatriculasOk, actual.MatriculasOk);
            mockIErpAcademicoServiceClient.Verify(s =>
                s.ValidateAlumnoMatriculacion(It.IsAny<ValidateAlumnoMatriculacionParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se obtiene las causas fallos matriculas Devuelve error")]
        public async Task GetCausasFallosComprobacionMatriculasDocumentacionErp_Excepcion()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            const string mensajeEsperado = "[ERP Académico]: No se ha podido obtener los datos de la Matriculación del Alumno.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s =>
                    s.ValidateAlumnoMatriculacion(It.IsAny<ValidateAlumnoMatriculacionParameters>()))
                .ReturnsAsync(null as ValidateAlumnoMatriculacionErpAcademicoModel);

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.GetCausasFallosComprobacionMatriculasDocumentacionErp("12", "13");
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region FiltrarAsignaturasExpedienteAlumno

        [Fact(DisplayName = "Cuando se filtra el expediente con todas asignaturas y con asignaturas se Devuelven todas las asignaturas")]
        public void FiltrarAsignaturasExpedienteAlumno_TodasAsignaturas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = true,
                ConAsignaturas = true,
                AsignaturasSuperadas = false,
                AsignaturasSuspensas = false,
                AsignaturasMatriculadas = false,
                AsignaturasNoPresentadas = false
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.NotEmpty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.NotEmpty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
        }

        [Fact(DisplayName = "Cuando se filtra el expediente con asignaturas y asignaturas superadas se devuelven solo las asignaturas superadas a true")]
        public void FiltrarAsignaturasExpedienteAlumno_FilterAsignaturasSuperadas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = false,
                ConAsignaturas = true,
                AsignaturasSuperadas = true,
                AsignaturasSuspensas = false,
                AsignaturasMatriculadas = false,
                AsignaturasNoPresentadas = false
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.NotEmpty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.Empty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
            Assert.DoesNotContain(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas, a => !a.Asignatura.DatosGestor.Superado);
            Assert.DoesNotContain(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas, a => !a.Asignatura.DatosGestor.Superado);
        }

        [Fact(DisplayName = "Cuando se filtra el expediente con asignaturas y asignaturas suspensas se Devuelven solo las asignaturas superadas a false")]
        public void FiltrarAsignaturasExpedienteAlumno_FilterAsignaturasSuspensas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);
            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = false,
                ConAsignaturas = true,
                AsignaturasSuperadas = false,
                AsignaturasSuspensas = true,
                AsignaturasMatriculadas = false,
                AsignaturasNoPresentadas = false
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.Empty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.NotEmpty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
            Assert.DoesNotContain(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas, a => a.Asignatura.DatosGestor.Superado);
            Assert.DoesNotContain(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas, a => a.Asignatura.DatosGestor.Superado);
        }

        [Fact(DisplayName = "Cuando se filtra el expediente con asignaturas y asignaturas matriculadas se devuelven solo las asignaturas con nota numerica igual a menos 12")]
        public void FiltrarAsignaturasExpedienteAlumno_FilterAsignaturasMatriculadas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const int asignaturasMatriculadas = -12;
            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = false,
                ConAsignaturas = true,
                AsignaturasSuperadas = false,
                AsignaturasSuspensas = false,
                AsignaturasMatriculadas = true,
                AsignaturasNoPresentadas = false
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.NotEmpty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.NotEmpty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
            Assert.DoesNotContain(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas, a => decimal.Parse(a.Asignatura.DatosGestor.NotaNumerica) != asignaturasMatriculadas);
            Assert.Contains(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas, a => decimal.Parse(a.Asignatura.DatosGestor.NotaNumerica) == asignaturasMatriculadas);
        }

        [Fact(DisplayName = "Cuando se filtra el expediente con asignaturas y asignaturas no presentadas se devuelven solo las asignaturas con nota numerica igual a menos 1")]
        public void FiltrarAsignaturasExpedienteAlumno_FilterAsignaturasNoPresentadas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            const int asignaturasNoPresentadas = -1;
            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = false,
                ConAsignaturas = true,
                AsignaturasSuperadas = false,
                AsignaturasSuspensas = false,
                AsignaturasMatriculadas = false,
                AsignaturasNoPresentadas = true
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.NotEmpty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.NotEmpty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
            Assert.DoesNotContain(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas, a => decimal.Parse(a.Asignatura.DatosGestor.NotaNumerica) != asignaturasNoPresentadas);
            Assert.Contains(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas, a => decimal.Parse(a.Asignatura.DatosGestor.NotaNumerica) == asignaturasNoPresentadas);
        }

        [Fact(DisplayName = "Cuando se filtra el expediente con asignaturas false no se devuelven asignaturas")]
        public void FiltrarAsignaturasExpedienteAlumno_SinAsignaturas()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var filterAsignaturasExpediente = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery
            {
                TodasAsignaturas = true,
                ConAsignaturas = false,
                AsignaturasSuperadas = false,
                AsignaturasSuspensas = false,
                AsignaturasMatriculadas = false,
                AsignaturasNoPresentadas = false
            };

            var expedienteAlumno = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = PreparacionAsignaturasPlanSuperado()
                    }
                },
                ElementosNoSuperados = new ElementoNoSuperadoErpAcademicoModel
                {
                    AsignaturasPlanNoSuperadas = PreparacionAsignaturasPlanNoSuperado()
                }
            };

            //ACT
            sut.FiltrarAsignaturasExpedienteAlumno(expedienteAlumno, filterAsignaturasExpediente);

            //ASSERT
            Assert.Empty(expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas);
            Assert.Empty(expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas);
        }

        private static List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel> PreparacionAsignaturasPlanSuperado()
        {
            return new()
            {
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel()
                {
                    Id = 1,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 1,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "10",
                            Superado = true
                        }
                    }
                },
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel()
                {
                    Id = 2,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 2,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "-1",
                            Superado = true
                        }
                    }
                },
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel()
                {
                    Id = 3,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 3,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "-12",
                            Superado = true
                        }
                    }
                }
            };
        }

        private static List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel> PreparacionAsignaturasPlanNoSuperado()
        {
            return new()
            {
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = 4,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 4,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "4",
                            Superado = false
                        }
                    }
                },
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = 5,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 5,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "-1",
                            Superado = false
                        }
                    }
                },
                new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = 6,
                    Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = 6,
                        DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                        {
                            NotaNumerica = "-12",
                            Superado = false
                        }
                    }
                }
            };
        }

        #endregion

        #region GetAsignaturaGestor

        [Fact(DisplayName = "Cuando no existe la asignatura gestor por el id asignatura plan Retorna null")]
        public void GetAsignaturaGestor_AsignaturaGestor_NoExiste()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.GetAsignaturaGestor(1,
                new List<AsignaturaErpAcademicoExpedientesIntegrationModel>());

            //ASSERT
            Assert.Null(actual);
        }

        [Fact(DisplayName = "Cuando existe una asignatura gestor por el id asignatura plan Retorna una entidad")]
        public void GetAsignaturaGestor_UnaAsignaturaGestor_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var datosGestor = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                new ()
                {
                    IdAsignatura = 1
                }
            };
            var asignaturaPlanId = 1;

            //ACT
            var actual = sut.GetAsignaturaGestor(asignaturaPlanId, datosGestor);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignaturaPlanId, actual.IdAsignatura);
        }

        [Fact(DisplayName = "Cuando existen varias asignaturas gestor por el id asignatura plan y hay una superada Retorna una entidad")]
        public void GetAsignaturaGestor_AsignaturaGestor_Superada_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturaPlanId = 1;
            var notaNumerica = 15;
            var datosGestor = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                new ()
                {
                    IdAsignatura = 1,
                    Superado = true,
                    NotaNumerica = 10
                },
                new ()
                {
                    IdAsignatura = 1,
                    Superado = true,
                    NotaNumerica = notaNumerica
                },
                new ()
                {
                    IdAsignatura = 1,
                    Superado = false,
                    NotaNumerica = 20
                }
            };

            //ACT
            var actual = sut.GetAsignaturaGestor(asignaturaPlanId, datosGestor);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignaturaPlanId, actual.IdAsignatura);
            Assert.Equal(notaNumerica, actual.NotaNumerica);
        }

        [Fact(DisplayName = "Cuando existen varias asignaturas gestor por el id asignatura plan y no están superadas Retorna una entidad")]
        public void GetAsignaturaGestor_AsignaturaGestor_NoSuperada_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);

            var asignaturaPlanId = 1;
            var notaNumerica = 20;
            var datosGestor = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                new ()
                {
                    IdAsignatura = 1,
                    Superado = false,
                    NotaNumerica = 10
                },
                new ()
                {
                    IdAsignatura = 1,
                    Superado = false,
                    NotaNumerica = notaNumerica
                },
                new ()
                {
                    IdAsignatura = 1,
                    Superado = false,
                    NotaNumerica = 15
                }
            };

            //ACT
            var actual = sut.GetAsignaturaGestor(asignaturaPlanId, datosGestor);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(asignaturaPlanId, actual.IdAsignatura);
            Assert.Equal(notaNumerica, actual.NotaNumerica);
        }

        #endregion

        #region RemoveAsignaturasGestorDuplicadas

        [Theory(DisplayName = "Cuando existen asignaturas repetidas y todas son superadas o no superadas Se obtiene sólo la válida y las demás repetidas se eliminan")]
        [InlineData(true)]
        [InlineData(false)]
        public void RemoveAsignaturasGestorDuplicadas_Ok(bool superada)
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var asignaturaValida = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                IdAsignatura = 1,
                Superado = superada,
                NotaNumerica = 20
            };

            sut.Setup(x => x.GetAsignaturaGestor(It.IsAny<int>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>())).Returns(asignaturaValida);

            var asignaturasGestor = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                new ()
                {
                    IdAsignatura = 1,
                    Superado = superada,
                    NotaNumerica = 10
                },
                new ()
                {
                    IdAsignatura = 1,
                    Superado = superada,
                    NotaNumerica = 15
                },
                asignaturaValida,
                new ()
                {
                    IdAsignatura = 2,
                    Superado = superada,
                    NotaNumerica = 20
                },
                new ()
                {
                    IdAsignatura = 3,
                    Superado = superada,
                    NotaNumerica = 20
                }
            };

            //ACT
            sut.Object.RemoveAsignaturasGestorDuplicadas(asignaturasGestor);

            //ASSERT
            Assert.NotNull(asignaturasGestor);
            Assert.Equal(3, asignaturasGestor.Count);
            sut.Verify(x => x.GetAsignaturaGestor(It.IsAny<int>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando existen asignaturas repetidas Se obtiene sólo la válida y las demás repetidas se eliminan")]
        public void RemoveAsignaturasGestorDuplicadas_Superadas_NoSuperadas_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>>
                {
                    CallBase = true
                };

            var mockIExpedientesGestorUnirServiceClient = new Mock<IExpedientesGestorUnirServiceClient>
            {
                CallBase = true
            };

            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler>(Context,
                mockIExpedientesGestorUnirServiceClient.Object, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            var asignaturaValida = new AsignaturaErpAcademicoExpedientesIntegrationModel
            {
                IdAsignatura = 1,
                Superado = true,
                NotaNumerica = 20
            };

            sut.Setup(x => x.GetAsignaturaGestor(It.IsAny<int>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>())).Returns(asignaturaValida);

            var asignaturasGestor = new List<AsignaturaErpAcademicoExpedientesIntegrationModel>
            {
                new ()
                {
                    IdAsignatura = 1,
                    Superado = true,
                    NotaNumerica = 10
                },
                asignaturaValida,
                new ()
                {
                    IdAsignatura = 1,
                    Superado = false,
                    NotaNumerica = 25
                },
                new ()
                {
                    IdAsignatura = 2,
                    Superado = false,
                    NotaNumerica = 20
                },
                new ()
                {
                    IdAsignatura = 3,
                    Superado = false,
                    NotaNumerica = 20
                }
            };

            //ACT
            sut.Object.RemoveAsignaturasGestorDuplicadas(asignaturasGestor);

            //ASSERT
            Assert.NotNull(asignaturasGestor);
            Assert.Equal(3, asignaturasGestor.Count);
            sut.Verify(x => x.GetAsignaturaGestor(It.IsAny<int>(),
                It.IsAny<List<AsignaturaErpAcademicoExpedientesIntegrationModel>>()), Times.Once);
        }

        #endregion
    }
}
