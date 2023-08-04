using AutoMapper;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos
{
    [Collection("CommonTestCollection")]
    public class GetPagedExpedientesAlumnosQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetPagedExpedientesAlumnosQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando existen registros Devuelve elementos")]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        public async Task Handle_Ok(int cantidad)
        {
            //ARRANGE
            var acronimoUniversidad = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddRangeAsync(Enumerable.Range(1, cantidad)
                .Select(e => new ExpedienteAlumno
                {
                    Id = e,
                    AcronimoUniversidad = acronimoUniversidad
                }));
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetPagedExpedientesAlumnoQuery
            {
                Limit = cantidad,
                Offset = 1
            };
            var mockLocalizer = new Mock<IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new Mock<GetPagedExpedientesAlumnoQueryHandler>(Context, _mapper, mockLocalizer.Object,
                mockIErpAcademicoServiceClient.Object, mockIMediator.Object)
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<GetApplyQueryExpedientesAlumnosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Context.ExpedientesAlumno.AsQueryable());
            sut.Setup(s =>
                    s.ProyectarIntegracionExpedientesAlumnosErp(It.IsAny<List<ExpedienteAlumnoPagedListItemDto>>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Range(1, cantidad).Select(e => new ExpedienteAlumnoPagedListItemDto
                {
                    Id = e,
                    AcronimoUniversidad = acronimoUniversidad,
                    CountAnotaciones = cantidad
                }).ToList);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.TotalElements);
            Assert.Equal(acronimoUniversidad, actual.Elements.First().AcronimoUniversidad);
            Assert.True(actual.Elements.All(a => a.CountAnotaciones == cantidad));
            mockIMediator.Verify(s => s.Send(It.IsAny<GetApplyQueryExpedientesAlumnosQuery>(), 
                It.IsAny<CancellationToken>()), Times.Once);
            sut.Verify(s => s.ProyectarIntegracionExpedientesAlumnosErp(
                It.IsAny<List<ExpedienteAlumnoPagedListItemDto>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se envían los parámetros de paginación Devuelve error")]
        public async Task Handle_Excepcion()
        {
            //ARRANGE
            var request = new GetPagedExpedientesAlumnoQuery();
            var mockLocalizer = new Mock<IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los campos offset y limit son obligatorios";
            mockLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new GetPagedExpedientesAlumnoQueryHandler(Context, _mapper, mockLocalizer.Object,
                mockIErpAcademicoServiceClient.Object, mockIMediator.Object);

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

        #region ProyectarIntegracionExpedientesAlumnosErp

        [Fact(DisplayName = "Cuando no se envían dtos de expedientes a proyectar Devuelve vacío")]
        public async Task ProyectarIntegracionExpedientesAlumnosErp_SinExpedientes()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new GetPagedExpedientesAlumnoQueryHandler(Context, _mapper, mockLocalizer.Object,
                mockIErpAcademicoServiceClient.Object, mockIMediator.Object);

            //ACT
            var actual = await sut.ProyectarIntegracionExpedientesAlumnosErp(new List<ExpedienteAlumnoPagedListItemDto>(),
                CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando no se encuentran expedientes en erp Devuelve lista sin cambios")]
        public async Task ProyectarIntegracionExpedientesAlumnosErp_ErpSinExpedientes()
        {
            //ARRANGE
            var mockLocalizer = new Mock<IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new GetPagedExpedientesAlumnoQueryHandler(Context, _mapper, mockLocalizer.Object,
                mockIErpAcademicoServiceClient.Object, mockIMediator.Object);

            mockIErpAcademicoServiceClient
                .Setup(s => s.GetExpedientesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ExpedienteAcademicoModel>().ToArray());

            var expedientes = new List<ExpedienteAlumnoPagedListItemDto>
            {
                new()
                {
                    Id = 1
                },
                new()
                {
                    Id = 2
                }
            };

            //ACT
            var actual = await sut.ProyectarIntegracionExpedientesAlumnosErp(
                expedientes, CancellationToken.None);

            //ASSERT
            Assert.True(actual.All(e => string.IsNullOrEmpty(e.UniversidadDisplayName)));
            Assert.True(actual.All(e => string.IsNullOrEmpty(e.CentroEstudioDisplayName)));
            Assert.True(actual.All(e => string.IsNullOrEmpty(e.TipoEstudioDisplayName)));
            Assert.True(actual.All(e => string.IsNullOrEmpty(e.TituloDisplayName)));
            mockIErpAcademicoServiceClient.Verify(s => s.GetExpedientesAsync(
                It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se proyectan los datos de erp Devuelve dtos")]
        public async Task ProyectarIntegracionExpedientesAlumnosErp_Ok()
        {
            //ARRANGE
            var expedienteAlumnoOk = new ExpedienteAlumnoPagedListItemDto
            {
                Id = 1,
                IdRefIntegracionAlumno = "10",
                IdRefPlan = "11"
            };
            var expedienteErpOk = new ExpedienteAcademicoModel
            {
                IdIntegracion = "1",
                Plan = new PlanAcademicoModel
                {
                    Id = 11,
                    DisplayName = Guid.NewGuid().ToString(),
                    Estudio = new EstudioAcademicoModel
                    {
                        DisplayName = Guid.NewGuid().ToString(),
                        Tipo = new TipoEstudioAcademicoModel
                        {
                            DisplayName = Guid.NewGuid().ToString()
                        },
                        AreaAcademica = new AreaAcademicaAcademicoModel
                        {
                            Centro = new CentroAcademicoModel
                            {
                                DisplayName = Guid.NewGuid().ToString(),
                                Universidad = new UniversidadAcademicoModel
                                {
                                    DisplayName = Guid.NewGuid().ToString()
                                }
                            }
                        }
                    },
                    Titulo = new TituloAcademicoModel
                    {
                        DisplayName = Guid.NewGuid().ToString()
                    }
                }
            };
            var expedienteAlumnoSinRegistroErp = new ExpedienteAlumnoPagedListItemDto
            {
                Id = 2,
                IdRefIntegracionAlumno = "20",
                IdRefPlan = "21"
            };

            var expedientesAlumnosDtos = new List<ExpedienteAlumnoPagedListItemDto>
            {
                expedienteAlumnoOk,
                expedienteAlumnoSinRegistroErp
            };

            var mockLocalizer = new Mock<IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient
                .Setup(s => s.GetExpedientesAsync(It.IsAny<string[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[]
                {
                    expedienteErpOk
                });
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new GetPagedExpedientesAlumnoQueryHandler(Context, _mapper, mockLocalizer.Object,
                mockIErpAcademicoServiceClient.Object, mockIMediator.Object);

            //ACT
            var actual = await sut.ProyectarIntegracionExpedientesAlumnosErp(expedientesAlumnosDtos,
                CancellationToken.None);

            //ASSERT
            var expedienteProyeccionOk = expedientesAlumnosDtos.First();
            var expedienteProyeccionError = expedientesAlumnosDtos.Last();
            Assert.NotEmpty(actual);
            Assert.Equal(expedientesAlumnosDtos.Count, actual.Count);
            Assert.Equal(expedienteProyeccionOk.UniversidadDisplayName, expedienteErpOk.Plan.Estudio.AreaAcademica.Centro.Universidad.DisplayName);
            Assert.Equal(expedienteProyeccionOk.CentroEstudioDisplayName, expedienteErpOk.Plan.Estudio.AreaAcademica.Centro.DisplayName);
            Assert.Equal(expedienteProyeccionOk.TipoEstudioDisplayName, expedienteErpOk.Plan.Estudio.Tipo.DisplayName);
            Assert.Equal(expedienteProyeccionOk.TituloDisplayName, expedienteErpOk.Plan.Titulo.DisplayName);
            Assert.Null(expedienteProyeccionError.UniversidadDisplayName);
            Assert.Null(expedienteProyeccionError.CentroEstudioDisplayName);
            Assert.Null(expedienteProyeccionError.TipoEstudioDisplayName);
            Assert.Null(expedienteProyeccionError.EstudioDisplayName);
            Assert.Null(expedienteProyeccionError.TituloDisplayName);
        }

        #endregion
    }
}
