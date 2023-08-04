using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Security;
using Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente
{
    [Collection("CommonTestCollection")]
    public class GetPagedSeguimientosExpedientesByIdExpedienteQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetPagedSeguimientosExpedientesByIdExpedienteQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Theory(DisplayName = "Cuando se encuentran seguimientos de expedientes Devuelve resultado")]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(35)]
        public async Task Handle_Ok(int cantidad)
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, cantidad).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First()
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = cantidad,
                Offset = 0
            };
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ApplyQuery(It.IsAny<IQueryable<SeguimientoExpediente>>(),
                    It.Is<GetPagedSeguimientosExpedientesByIdExpedienteQuery>(i => i == request)))
                .Returns(Context.SeguimientosExpediente.AsQueryable);
            var expedientesEsperados = await
                Context.SeguimientosExpediente.Select(se => new SeguimientoExpedienteByIdExpedienteListItemDto
                {
                    Id = se.Id,
                    TipoSeguimiento = new TipoSeguimientoExpedienteByExpedienteListItemDto
                    {
                        Id = se.TipoSeguimiento.Id,
                        Nombre = se.TipoSeguimiento.Nombre
                    }
                }).ToListAsync(CancellationToken.None);
            sut.Setup(s =>
                    s.ProyectarIntegracionDatosErp(It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()))
                .ReturnsAsync(expedientesEsperados);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.TotalElements);
            sut.Verify(s => s.ApplyQuery(It.IsAny<IQueryable<SeguimientoExpediente>>(),
                It.Is<GetPagedSeguimientosExpedientesByIdExpedienteQuery>(i => i == request)), Times.Once);
            sut.Verify(s =>
                    s.ProyectarIntegracionDatosErp(It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()),
                Times.Once);
        }

        [Fact(DisplayName = "Cuando no se envía información de paginación Devuelve excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery();
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los campos offset y limit son obligatorios";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de id expediente Devuelve registros")]
        public async Task ApplyQuery_FilterIdExpedienteAlumno()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First()
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdExpedienteAlumno = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.Id == 1));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id tipo seguimiento Devuelve registros")]
        public async Task ApplyQuery_FilterIdTipoSeguimientoExpediente()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First()
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdTipoSeguimiento = TipoSeguimientoExpediente.ExpedienteCreado
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.TipoSeguimiento.Id == TipoSeguimientoExpediente.ExpedienteCreado));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de fechas Devuelve registros")]
        public async Task ApplyQuery_FilterFechaDesdeHasta()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First(),
                    Fecha = new DateTime(2021, 1, 8 + i)
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaDesde = new DateTime(2021, 1, 8),
                FilterFechaHasta = new DateTime(2021, 1, 10)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(2, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a =>
                a.Fecha <= request.FilterFechaHasta && a.Fecha >= request.FilterFechaDesde));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de fecha desde Devuelve registros")]
        public async Task ApplyQuery_FilterFechaDesde()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First(),
                    Fecha = new DateTime(2021, 1, 8 + i)
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaDesde = new DateTime(2021, 1, 8)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.Fecha >= request.FilterFechaDesde));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de fecha hasta Devuelve registros")]
        public async Task ApplyQuery_FilterFechaHasta()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First(),
                    Fecha = new DateTime(2021, 1, 7 + i)
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaHasta = new DateTime(2021, 1, 10)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.Fecha <= request.FilterFechaHasta));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de descripción Devuelve registros")]
        public async Task ApplyQuery_FilterDescripcion()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = Context.ExpedientesAlumno.First(),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesByIdExpedienteQuery
            {
                Limit = 10,
                Offset = 1,
                FilterDescripcion = "1as"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.Descripcion.Contains("1as")));
        }

        #endregion

        #region ProyectarIntegracionDatosErp

        [Fact(DisplayName = "Cuando no se envían expedientes para integrar Devuelve vacío")]
        public async Task ProyectarIntegracionDatosErp_SinSeguimientos()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = await
                sut.ProyectarIntegracionDatosErp(new List<SeguimientoExpedienteByIdExpedienteListItemDto>());

            //ASSERT
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando se envían expedientes para integrar Devuelve lista")]
        public async Task ProyectarIntegracionDatosErp_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()))
                .ReturnsAsync(new List<UsuarioModel>());

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteByIdExpedienteListItemDto
            {
                Id = i,
                IdRefTrabajador = i.ToString()
            }).ToList();

            //ACT
            var actual = await
                sut.Object.ProyectarIntegracionDatosErp(seguimientosDtos);

            //ASSERT
            sut.Verify(s => s.GetUsuariosInternos(
                    It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()), Times.Once);
            Assert.Equal(seguimientosDtos.Count, actual.Count);
        }

        [Fact(DisplayName = "Cuando se envían expedientes para integrar y no existen usuarios Devuelve lista vacía")]
        public async Task ProyectarIntegracionDatosErp_SinUsuarios_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new Mock<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()))
                .ReturnsAsync(new List<UsuarioModel>());

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteByIdExpedienteListItemDto
            {
                Id = i
            }).ToList();

            //ACT
            var actual = await
                sut.Object.ProyectarIntegracionDatosErp(seguimientosDtos);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.GetPersonas(It.IsAny<string[]>()), Times.Never);
            sut.Verify(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteByIdExpedienteListItemDto>>()),
                Times.Once);
            Assert.Equal(seguimientosDtos.Count, actual.Count);
        }

        #endregion

        #region GetUsuariosInternos

        [Fact(DisplayName = "Cuando se obtienen los usuarios internos de erp Devuelve lista")]
        public async Task GetUsuariosInternos_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()))
                .ReturnsAsync(new UsuarioModel());

            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteByIdExpedienteListItemDto
            {
                Id = i,
                IdCuentaSeguridad = Guid.NewGuid().ToString()
            }).ToList();

            //ACT
            var actual = await
                sut.GetUsuariosInternos(seguimientosDtos);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()),
                Times.Exactly(seguimientosDtos.Count));
            Assert.Equal(seguimientosDtos.Count, actual.Count);
        }

        [Fact(DisplayName = "Cuando se obtienen los usuarios internos de erp Devuelve lista")]
        public async Task GetUsuariosInternos_UsuarioNoEncontrado()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()))
                .ReturnsAsync(null as UsuarioModel);

            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteByIdExpedienteListItemDto
            {
                Id = i,
                IdCuentaSeguridad = Guid.NewGuid().ToString()
            }).ToList();

            //ACT
            var actual = await
                sut.GetUsuariosInternos(seguimientosDtos);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()),
                Times.Exactly(seguimientosDtos.Count));
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando se obtienen los usuarios internos de erp Devuelve lista vacía")]
        public async Task GetUsuariosInternos_Vacio()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteByIdExpedienteListItemDto
            {
                Id = i
            }).ToList();

            //ACT
            var actual = await
                sut.GetUsuariosInternos(seguimientosDtos);

            //ASSERT
            mockIErpAcademicoServiceClient.Verify(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()),
                Times.Never);
            Assert.Empty(actual);
        }

        #endregion
    }
}
