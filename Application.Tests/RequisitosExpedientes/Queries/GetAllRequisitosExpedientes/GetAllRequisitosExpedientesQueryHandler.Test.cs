using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetAllRequisitosExpedientes;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Queries.GetAllRequisitosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetAllRequisitosExpedientesQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetAllRequisitosExpedientesQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se filtra por nombre Devuelve registros")]
        public async Task Handle_Nombre_Ok()
        {
            //ARRANGE
            var nombre = Guid.NewGuid().ToString();
            var request = new GetAllRequisitosExpedientesQuery
            {
                FilterNombre = nombre
            };
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>()
            {
                CallBase = true
            };
            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>())).Returns(Task.CompletedTask);
            var requisitos = Enumerable.Range(1, 10)
                .Select(r => new RequisitoExpediente
                {
                    Id = r,
                    Nombre = $"{nombre} - {r}"
                }).ToList();

            await Context.RequisitosExpedientes.AddRangeAsync(requisitos);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(requisitos.Count, actual.Length);
            sutMock.Verify(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando no se filtra por nombre Devuelve registros")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new GetAllRequisitosExpedientesQuery();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>()
            {
                CallBase = true
            };
            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>())).Returns(Task.CompletedTask);
            var requisitos = Enumerable.Range(1, 10)
                .Select(r => new RequisitoExpediente
                {
                    Id = r,
                    Nombre = $"{Guid.NewGuid()} - {r}"
                }).ToList();

            await Context.RequisitosExpedientes.AddRangeAsync(requisitos);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(requisitos.Count, actual.Length);
            sutMock.Verify(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>()), Times.Once());
        }

        [Theory(DisplayName = "Cuando existen Requisitos expedientes sin filtrar por nombre Devuelve resultado paginado")]
        [InlineData(5)]
        [InlineData(10)]
        public async Task Handle_Paginado_SinFiltroNombre(int cantidad)
        {
            //ARRANGE
            var request = new GetAllRequisitosExpedientesQuery
            {
                Offset = 0,
                Limit = cantidad
            };
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>()
            {
                CallBase = true
            };
            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>())).Returns(Task.CompletedTask);
            var requisitos = Enumerable.Range(1, cantidad)
                .Select(r => new RequisitoExpediente
                {
                    Id = r,
                    Nombre = $"{Guid.NewGuid()} - {r}"
                }).ToList();

            await Context.RequisitosExpedientes.AddRangeAsync(requisitos);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(cantidad, actual.Length);
            sutMock.Verify(x => x.GetModosRequerimientoDocumentacion(
                It.IsAny<RequisitosExpedientesListItemDto[]>()), Times.Once());
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
            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            var requisitos = new RequisitosExpedientesListItemDto[] { };

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
            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            var requisitos = new RequisitosExpedientesListItemDto[]
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

            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            var requisitos = new RequisitosExpedientesListItemDto[]
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

            var sutMock = new Mock<GetAllRequisitosExpedientesQueryHandler>(Context, _mapper, mockIErpAcademicoServiceClient.Object)
            {
                CallBase = true
            };
            var requisitos = new RequisitosExpedientesListItemDto[]
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
