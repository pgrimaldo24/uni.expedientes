using AutoMapper;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes
{
    [Collection("CommonTestCollection")]
    public class GetPagedComportamientosExpedientesQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetPagedComportamientosExpedientesQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando no se envian los datos de paginación Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new GetPagedComportamientosExpedientesQuery();
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedComportamientosExpedientesQueryHandler(Context, _mapper,
                mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        [Fact(DisplayName = "Cuando se obtienen los comportamientos Devuelve una lista")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new GetPagedComportamientosExpedientesQuery
            {
                Offset = -1,
                Limit = 10
            };
            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new ComportamientoExpediente
            {
                Id = r
            }));
            await Context.SaveChangesAsync();
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetPagedComportamientosExpedientesQueryHandler>(Context, 
                _mapper, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            sutMock.Setup(x => x.ApplyQuery(It.IsAny<IQueryable<ComportamientoExpediente>>(),
                It.IsAny<GetPagedComportamientosExpedientesQuery>())).Returns(Context.ComportamientosExpedientes.AsQueryable());

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual.Elements);
            sutMock.Verify(x => x.ApplyQuery(It.IsAny<IQueryable<ComportamientoExpediente>>(),
                It.IsAny<GetPagedComportamientosExpedientesQuery>()), Times.Once);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando se aplica el filtro de nombre Devuelve Registros")]
        public async Task ApplyQuery_Nombre_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedComportamientosExpedientesQueryHandler(Context, 
                _mapper, mockIStringLocalizer.Object);

            var nombre = Guid.NewGuid().ToString();
            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new ComportamientoExpediente
            {
                Id = r,
                Nombre = $"{r}-{nombre}"
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedComportamientosExpedientesQuery
            {
                Nombre = $"{1}-{nombre}"
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ComportamientosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Single(actual);
            Assert.True(actual.Any(r => r.Nombre == $"{1}-{nombre}"));
        }


        [Fact(DisplayName = "Cuando se aplica el filtro de está vigente Devuelve Registros")]
        public async Task ApplyQuery_EstaVigente_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedComportamientosExpedientesQueryHandler(Context, 
                _mapper, mockIStringLocalizer.Object);

            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new ComportamientoExpediente
            {
                Id = r,
                EstaVigente = true
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedComportamientosExpedientesQuery
            {
                EstaVigente = true
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ComportamientosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.EstaVigente));
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de nivel de uso Devuelve Registros")]
        public async Task ApplyQuery_NivelUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedComportamientosExpedientesQueryHandler(Context,
                _mapper, mockIStringLocalizer.Object);

            var cantidad = 10;
            const string nombre = "UNIR";
            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, cantidad).Select(s => new ComportamientoExpediente
            {
                Id = s,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = s,
                        AcronimoUniversidad = "UNIR" + s,
                        NombreTipoEstudio = "UNIR" + s,
                        NombreEstudio = "UNIR" + s,
                        NombrePlan = "UNIR" + s,
                        IdRefUniversidad = "UNIR" + s,
                        IdRefTipoEstudio = "UNIR" + s,
                        IdRefEstudio = "UNIR" + s,
                        IdRefPlan = "UNIR" + s,
                        IdRefTipoAsignatura =  "UNIR" + s,
                        NombreTipoAsignatura =  "UNIR" + s,
                        IdRefAsignaturaPlan = "UNIR" + s,
                        IdRefAsignatura = "UNIR" + s,
                        NombreAsignatura = "UNIR" + s,
                    }
                }
            }));
            await Context.SaveChangesAsync();

            var request = new GetPagedComportamientosExpedientesQuery
            {
                NivelUso = nombre
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ComportamientosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(cantidad, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se aplica el filtro de ids de condiciones Devuelve Registros")]
        public async Task ApplyQuery_IdsCondiciones_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler>>
            {
                CallBase = true
            };
            var sut = new GetPagedComportamientosExpedientesQueryHandler(Context,
                _mapper, mockIStringLocalizer.Object);
            
            await Context.ComportamientosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new ComportamientoExpediente
            {
                Id = r,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpediente
                        {
                            Id = r
                        }
                    }
                }
            }));
            await Context.SaveChangesAsync();

            var idsCondiciones = new List<int> { 1, 2, 3 };
            var request = new GetPagedComportamientosExpedientesQuery
            {
                IdsCondiciones = idsCondiciones
            };

            //ACT
            var actual = sut.ApplyQuery(Context.ComportamientosExpedientes.AsQueryable(), request);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.True(actual.All(r => r.RequisitosComportamientosExpedientes
                .Any(rce => idsCondiciones.Contains(rce.RequisitoExpediente.Id))));
        }

        #endregion
    }
}
