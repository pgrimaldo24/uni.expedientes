using AutoMapper;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetPagedRequisitosExpedientesQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetPagedRequisitosExpedientesQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }
        #region Handle

        [Fact(DisplayName = "Cuando no se envian los datos de paginación Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new GetPagedRequisitosExpedientesQuery();
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        [Fact(DisplayName = "Cuando se obtienen los requisitos Devuelve una lista")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new GetPagedRequisitosExpedientesQuery
            {
                Offset = -1,
                Limit = 10
            };
            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            sutMock.Setup(x => x.ApplyQuery(It.IsAny<IQueryable<RequisitoExpediente>>(),
                It.IsAny<GetPagedRequisitosExpedientesQuery>())).Returns(Context.RequisitosExpedientes.AsQueryable());
            sutMock.Setup(x => x.GetModosRequerimientoDocumentacion(
                    It.IsAny<List<RequisitosExpedientesListItemDto>>())).Returns(Task.CompletedTask);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual.Elements);
            sutMock.Verify(x => x.ApplyQuery(It.IsAny<IQueryable<RequisitoExpediente>>(),
                It.IsAny<GetPagedRequisitosExpedientesQuery>()), Times.Once);
            sutMock.Verify(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<List<RequisitosExpedientesListItemDto>>()), Times.Once);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de nombre Devuelve Registros")]
        public async Task ApplyQuery_Nombre_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            var nombre = Guid.NewGuid().ToString();
            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                Nombre = $"{r}-{nombre}",
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                Nombre = $"{1}-{nombre}"
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(r => r.Nombre == $"{1}-{nombre}"));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de está vigente Devuelve Registros")]
        public async Task ApplyQuery_EstaVigente_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstaVigente = true,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                EstaVigente = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.EstaVigente));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de requerido para título Devuelve Registros")]
        public async Task ApplyQuery_RequeridoTitulo_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                RequeridaParaTitulo = true,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                RequeridoTitulo = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequeridaParaTitulo));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de requiere matricularse Devuelve Registros")]
        public async Task ApplyQuery_RequiereMatricularse_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                },
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>
                {
                    new()
                    {
                        Id = r,
                        RequiereMatricularse = true
                    }
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                RequiereMatricularse = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequisitosExpedientesRequerimientosTitulos
                .All(rert => rert.RequiereMatricularse)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de requerido para pago de tasas Devuelve Registros")]
        public async Task ApplyQuery_RequeridoParaPago_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                RequeridaParaPago = true,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                RequeridoParaPago = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequeridaParaPago));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de estados de expedientes Devuelve Registros")]
        public async Task ApplyQuery_IdsEstadosExpedientes_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();
            var idsEstados = new List<int> { 1, 2, 3 };
            var request = new GetPagedRequisitosExpedientesQuery
            {
                IdsEstadosExpedientes = idsEstados
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => idsEstados.Contains(r.EstadoExpediente.Id)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de requiere documentación Devuelve Registros")]
        public async Task ApplyQuery_RequiereDocumentacion_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                RequiereDocumentacion = true,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                RequiereDocumentacion = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequiereDocumentacion));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de documentación protegida Devuelve Registros")]
        public async Task ApplyQuery_DocumentacionProtegida_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                },
                RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>
                {
                    new()
                    {
                        Id = r,
                        DocumentoSecurizado = true
                    }
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedRequisitosExpedientesQuery
            {
                DocumentacionProtegida = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequisitosExpedientesDocumentos
                .All(red => red.DocumentoSecurizado)));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de modos requerimiento documentación Devuelve Registros")]
        public async Task ApplyQuery_IdsModosRequerimientoDocumentacion_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedRequisitosExpedientesQueryHandler(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object);

            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = r
                },
                IdRefModoRequerimientoDocumentacion = r
            }));
            await Context.SaveChangesAsync();
            var idsModosRequerimientoDocumentacion = new List<int> { 1, 2, 3 };
            var request = new GetPagedRequisitosExpedientesQuery
            {
                IdsModosRequerimientoDocumentacion = idsModosRequerimientoDocumentacion
            };

            //ACT
            var actual = sut.ApplyQuery(Context.RequisitosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => idsModosRequerimientoDocumentacion.Contains(r.IdRefModoRequerimientoDocumentacion.Value)));
        }

        #endregion

        #region GetModosRequerimientoDocumentacion

        [Fact(DisplayName = "Cuando no hay requisitos Termina el proceso")]
        public async Task GetModosRequerimientoDocumentacion_Requisitos_Empty()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            { CallBase = true };
            var requisitos = new List<RequisitosExpedientesListItemDto>();

            //ACT
            await sutMock.Object.GetModosRequerimientoDocumentacion(requisitos);

            //ASSERT
            Assert.Empty(requisitos);
        }

        [Fact(DisplayName = "Cuando los valores de IdRefModoRequerimientoDocumentacion son null Termina el proceso")]
        public async Task GetModosRequerimientoDocumentacion_IdRefModoRequerimientoDocumentacion_Null()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            { CallBase = true };
            var requisitos = new List<RequisitosExpedientesListItemDto>
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
            await sutMock.Object.GetModosRequerimientoDocumentacion(requisitos);

            //ASSERT
            Assert.True(requisitos.All(r => !r.IdRefModoRequerimientoDocumentacion.HasValue));
        }

        [Fact(DisplayName = "Cuando el servicio externo no devuelve información Termina el proceso")]
        public async Task GetModosRequerimientoDocumentacion_Servicio_Empty()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(x =>
                    x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()))
                .ReturnsAsync(new List<ModoRequerimientoDocumentacionAcademicoModel>());

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            { CallBase = true };
            var requisitos = new List<RequisitosExpedientesListItemDto>
            {
                new()
                {
                    Id = 1,
                    IdRefModoRequerimientoDocumentacion = 1
                },
                new()
                {
                    Id = 2,
                    IdRefModoRequerimientoDocumentacion = 2
                }
            };

            //ACT
            await sutMock.Object.GetModosRequerimientoDocumentacion(requisitos);

            //ASSERT
            Assert.True(requisitos.All(r => string.IsNullOrEmpty(r.ModoRequerimientoDocumentacion)));
            mockIErpAcademicoServiceClient.Verify(x =>
                x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se obtienen los nombres de los modos de requerimiento documentación Devuelve Ok")]
        public async Task GetModosRequerimientoDocumentacion_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            mockIErpAcademicoServiceClient.Setup(x =>
                    x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()))
                .ReturnsAsync(new List<ModoRequerimientoDocumentacionAcademicoModel>
                {
                    new()
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString()
                    },
                    new()
                    {
                        Id = 2,
                        Nombre = Guid.NewGuid().ToString()
                    }
                });

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedRequisitosExpedientesQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object, mockIStringLocalizer.Object)
            { CallBase = true };
            var requisitos = new List<RequisitosExpedientesListItemDto>
            {
                new()
                {
                    Id = 1,
                    IdRefModoRequerimientoDocumentacion = 1
                },
                new()
                {
                    Id = 2,
                    IdRefModoRequerimientoDocumentacion = 2
                }
            };

            //ACT
            await sutMock.Object.GetModosRequerimientoDocumentacion(requisitos);

            //ASSERT
            Assert.True(requisitos.All(r => !string.IsNullOrEmpty(r.ModoRequerimientoDocumentacion)));
            mockIErpAcademicoServiceClient.Verify(x =>
                x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()), Times.Once);
        }

        #endregion
    }
}
