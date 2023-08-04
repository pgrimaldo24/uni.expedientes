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
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Security;
using Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetPagedSeguimientosExpedientesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetPagedSeguimientosExpedientesQueryHandlerTest(CommonTestFixture fixture)
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new Mock<GetPagedSeguimientosExpedientesQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sut.Setup(s => s.ApplyQuery(It.IsAny<IQueryable<SeguimientoExpediente>>(),
                    It.Is<GetPagedSeguimientosExpedientesQuery>(i => i == request)))
                .Returns(Context.SeguimientosExpediente.AsQueryable);
            var expedientesEsperados = await
                Context.SeguimientosExpediente.Select(se => new SeguimientoExpedienteListItemDto
                {
                    Id = se.Id,
                    TipoSeguimiento = new TipoSeguimientoExpedienteListItemDto
                    {
                        Id = se.TipoSeguimiento.Id,
                        Nombre = se.TipoSeguimiento.Nombre
                    },
                    ExpedienteAlumno = new ExpedienteAlumnoListItemDto
                    {
                        Id = se.ExpedienteAlumno.Id
                    }
                }).ToListAsync(CancellationToken.None);
            sut.Setup(s => s.ProyectarIntegracionDatosErp(It.IsAny<List<SeguimientoExpedienteListItemDto>>()))
                .ReturnsAsync(expedientesEsperados);

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(cantidad, actual.Elements.Count);
            sut.Verify(s => s.ApplyQuery(It.IsAny<IQueryable<SeguimientoExpediente>>(),
                It.Is<GetPagedSeguimientosExpedientesQuery>(i => i == request)), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se envía información de paginación Devuelve excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new GetPagedSeguimientosExpedientesQuery();
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
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
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdExpedienteAlumno = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.Id == 1));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id integración alumno Devuelve registros")]
        public async Task ApplyQuery_FilterIdIntegracionAlumno()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123"
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdRefIntegracionAlumno = "123"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.IdRefIntegracionAlumno == "123"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id plan Devuelve registros")]
        public async Task ApplyQuery_FilterIdPlan()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefPlan = "123"
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdRefPlan = 123
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.IdRefPlan == "123"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id tipo seguimiento Devuelve registros")]
        public async Task ApplyQuery_FilterIdTipoSeguimiento()
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdTipoSeguimiento = TipoSeguimientoExpediente.ExpedienteCreado
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaDesde = new DateTime(2021, 1, 8),
                FilterFechaHasta = new DateTime(2021, 1, 10)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaDesde = new DateTime(2021, 1, 8)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
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
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterFechaHasta = new DateTime(2021, 1, 10)
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(3, await actual.CountAsync(CancellationToken.None));
            Assert.True(await actual.AllAsync(a => a.Fecha <= request.FilterFechaHasta));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id cuenta seguridad Devuelve registros")]
        public async Task ApplyQuery_FilterIdRefTrabajador_IdCuentaSeguridad()
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
                    IdCuentaSeguridad = $"12{i}"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdCuentaSeguridad = "121"
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.IdCuentaSeguridad == "121"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id ref universidad Devuelve registros")]
        public async Task ApplyQuery_FilterIdRefUniversidad()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12"
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "13"
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdRefUniversidad = 12
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.IdRefUniversidad.Contains("12")));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de nombre del alumno Devuelve registros")]
        public async Task ApplyQuery_FilterNombreAlumno()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            var nombreAlumnoBuscar = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12",
                AlumnoNombre = nombreAlumnoBuscar
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "13",
                AlumnoNombre = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterNombreAlumno = nombreAlumnoBuscar
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.AlumnoNombre.StartsWith(nombreAlumnoBuscar)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de primer apellido del alumno Devuelve registros")]
        public async Task ApplyQuery_FilterPrimerApellido()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            var primerApellidoBuscar = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12",
                AlumnoApellido1 = primerApellidoBuscar
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "13",
                AlumnoApellido1 = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterPrimerApellido = primerApellidoBuscar
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.AlumnoApellido1.StartsWith(primerApellidoBuscar)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de segundo apellido del alumno Devuelve registros")]
        public async Task ApplyQuery_FilterSegundoApellido()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            var segundoApellidoBuscar = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12",
                AlumnoApellido2 = segundoApellidoBuscar
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "13",
                AlumnoApellido2 = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterSegundoApellido = segundoApellidoBuscar
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.AlumnoApellido2.StartsWith(segundoApellidoBuscar)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de número de identificación Devuelve registros")]
        public async Task ApplyQuery_FilterNroDocIdentificacion()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            var numeroIdentificacionAlumno = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = numeroIdentificacionAlumno
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "13",
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterNroDocIdentificacion = numeroIdentificacionAlumno
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.AlumnoNroDocIdentificacion.StartsWith(numeroIdentificacionAlumno)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de id integración Devuelve registros")]
        public async Task ApplyQuery_FilterAlumno_IdRefIntegracionAlumno()
        {
            //ARRANGE
            await Context.TipoSeguimientosExpediente.AddAsync(new TipoSeguimientoExpediente
            {
                Id = TipoSeguimientoExpediente.ExpedienteCreado,
                Nombre = Guid.NewGuid().ToString()
            });
            var idRefIntegracionAlumno = Guid.NewGuid().ToString();
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 1,
                IdRefUniversidad = "12",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString(),
                IdRefIntegracionAlumno = idRefIntegracionAlumno
            });
            await Context.ExpedientesAlumno.AddAsync(new ExpedienteAlumno
            {
                Id = 10,
                IdRefUniversidad = "12",
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString()
            });
            await Context.SaveChangesAsync(CancellationToken.None);
            await Context.SeguimientosExpediente.AddRangeAsync(Enumerable.Range(1, 3).Select(i =>
                new SeguimientoExpediente
                {
                    Id = i,
                    TipoSeguimiento = Context.TipoSeguimientosExpediente.First(),
                    ExpedienteAlumno = i % 2 == 0
                        ? Context.ExpedientesAlumno.First(ea => ea.Id == 1)
                        : Context.ExpedientesAlumno.First(ea => ea.Id == 10),
                    Descripcion = $"{i}asd"
                }));
            await Context.SaveChangesAsync(CancellationToken.None);
            var request = new GetPagedSeguimientosExpedientesQuery
            {
                Limit = 10,
                Offset = 1,
                FilterIdRefIntegracionAlumno = idRefIntegracionAlumno
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.SeguimientosExpediente.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(await actual.AllAsync(a => a.ExpedienteAlumno.IdRefIntegracionAlumno.Equals(idRefIntegracionAlumno)));
        }

        #endregion

        #region ProyectarIntegracionDatosErp

        [Fact(DisplayName = "Cuando no se envían expedientes para integrar Devuelve vacío")]
        public async Task ProyectarIntegracionDatosErp_SinSeguimientos()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            //ACT
            var actual = await
                sut.ProyectarIntegracionDatosErp(new List<SeguimientoExpedienteListItemDto>());

            //ASSERT
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando se envían expedientes para integrar Devuelve lista")]
        public async Task ProyectarIntegracionDatosErp_Ok()
        {
            //ARRANGE
            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteListItemDto
            {
                IdRefTrabajador = Guid.NewGuid().ToString(),
                Id = i,
                ExpedienteAlumno = new ExpedienteAlumnoListItemDto
                {
                    Id = 1
                }
            }).ToList();

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetPersonas(It.IsAny<string[]>()))
                .ReturnsAsync(Array.Empty<PersonaAcademicoModel>());
            var sut = new Mock<GetPagedSeguimientosExpedientesQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteListItemDto>>()))
                .ReturnsAsync(new List<UsuarioModel>());

            //ACT
            var actual = await
                sut.Object.ProyectarIntegracionDatosErp(seguimientosDtos);

            //ASSERT
            Assert.Equal(seguimientosDtos.Count, actual.Count);
            sut.Verify(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteListItemDto>>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando se envían expedientes para integrar y no se encuentran usuarios Devuelve lista vacía")]
        public async Task ProyectarIntegracionDatosErp_SinUsuarios()
        {
            //ARRANGE
            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteListItemDto
            {
                Id = i,
                ExpedienteAlumno = new ExpedienteAlumnoListItemDto
                {
                    Id = 1
                }
            }).ToList();

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetPersonas(It.IsAny<string[]>()))
                .ReturnsAsync(Array.Empty<PersonaAcademicoModel>());
            var sut = new Mock<GetPagedSeguimientosExpedientesQueryHandler>(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };

            sut.Setup(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteListItemDto>>()))
                .ReturnsAsync(new List<UsuarioModel>());

            //ACT
            var actual = await sut.Object.ProyectarIntegracionDatosErp(seguimientosDtos);

            //ASSERT
            Assert.Equal(seguimientosDtos.Count, actual.Count);
            sut.Verify(s => s.GetUsuariosInternos(It.IsAny<List<SeguimientoExpedienteListItemDto>>()), Times.Once());
        }

        #endregion

        #region GetUsuariosInternos

        [Fact(DisplayName = "Cuando se obtienen los usuarios internos de erp Devuelve lista")]
        public async Task GetUsuariosInternos_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer =
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()))
                .ReturnsAsync(new UsuarioModel());

            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteListItemDto
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
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            mockIErpAcademicoServiceClient.Setup(s => s.GetUserNameByIdSeguridad(It.IsAny<string>()))
                .ReturnsAsync(null as UsuarioModel);

            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteListItemDto
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
                new Mock<IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler>>
                {
                    CallBase = true
                };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sut = new GetPagedSeguimientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object, mockIErpAcademicoServiceClient.Object);

            var seguimientosDtos = Enumerable.Range(1, 3).Select(i => new SeguimientoExpedienteListItemDto
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
