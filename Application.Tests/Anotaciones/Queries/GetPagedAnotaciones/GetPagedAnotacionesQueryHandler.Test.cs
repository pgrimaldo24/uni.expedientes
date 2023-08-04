using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById;
using Unir.Expedientes.Application.Anotaciones.Queries.GetPagedAnotaciones;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Models.Results;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;


namespace Unir.Expedientes.Application.Tests.Anotaciones.Queries.GetPagedAnotaciones
{
    [Collection("CommonTestCollection")]
    public class GetPagedAnotacionesQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;
        public GetPagedAnotacionesQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando existe por lo menos un registro devuelve una lista con datos")]
        public async Task Handle_ExisteElementos()
        {
            //ARRANGE
            var cantidadEsperada = 1;
            var role = AppConfiguration.KeyAdminRole;
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);

            var anotacion = new Anotacion
            {
                Id = 1,
                RolesAnotaciones = new List<RolAnotacion>
                {
                    new ()
                    {
                        Rol = Guid.NewGuid().ToString()
                    },
                    new ()
                    {
                        Rol = Guid.NewGuid().ToString()
                    }
                },
                ExpedienteAlumno = expedienteAlumno
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();
            var mockIdentityService = new Mock<IIdentityService>();
            mockIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(new IdentityModel
            {
                Roles = new[] { role }
            });

            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                Roles = new [] { role }
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountInfo);
            var sut = new Mock<GetPagedAnotacionesQueryHandler>(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object)
            { CallBase = true };

            //ACT
            var actual = await sut.Object.Handle(new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1
            }, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(cantidadEsperada, actual.Elements.Count);
            Assert.Equal(anotacion.RolesAnotaciones.Count, Context.Anotaciones.First().RolesAnotaciones.Count);
            mockIdentityService.Verify(s => s.GetUserIdentityInfo(), Times.Once);
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se encuentran una anotación devuelve un listado vacio")]
        public async Task Handle_NoExistesElementos()
        {
            //ARRANGE
            var cantidadEsperada = 0;
            var role = AppConfiguration.KeyAdminRole;
            var mockIdentityService = new Mock<IIdentityService>();
            mockIdentityService.Setup(s => s.GetUserIdentityInfo()).Returns(new IdentityModel
            {
                Roles = new[] { role }
            });

            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                Roles = new[] { role }
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountInfo);
            var sut = new Mock<GetPagedAnotacionesQueryHandler>(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object)
            { CallBase = true };

            //ACT
            var actual = await sut.Object.Handle(new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1
            }, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.Equal(cantidadEsperada, actual.Elements.Count);
        }

        [Fact(DisplayName = "Cuando no se encuentra la información del usuario devuelve una lista vacía")]
        public async Task Handle_RetornoListaInforSecuritySinElementos()
        { 
            //ARRANGE 
            var mockIdentityService = new Mock<IIdentityService> { CallBase = true }; 
            var pagedQuery = new GetPagedAnotacionesQuery();
            var identityModel = new IdentityModel();  
          
            mockIdentityService.Setup(x => x.GetUserIdentityInfo()).Returns(null as IdentityModel);
                var mockSecurityService = new Mock<ISecurityService> { CallBase = true }; 
            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object,
                mockSecurityService.Object); 
            //ACT 
            await sut.Handle(pagedQuery, CancellationToken.None);  
            //ASSERT
            Assert.Null(identityModel.Id); 
            mockIdentityService.Verify(t => t.GetUserIdentityInfo(), Times.Once); 
        }

        [Fact(DisplayName = "Cuando no se encuentra la cuenta del usuario devuelve una lista vacía")]
        public async Task Handle_RetornoListaAccountSecuritySinElementos()
        {
            //ARRANGE 
            var pagedQuery = new GetPagedAnotacionesQuery();
            var mockIdentityService = new Mock<IIdentityService> { CallBase = true };  
            mockIdentityService.Setup(x => x.GetUserIdentityInfo())
                .Returns(new  IdentityModel()); 
            var mockSecurityService = new Mock<ISecurityService> { CallBase = true };
            mockSecurityService.Setup(c => c.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(null as AccountModel);

            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object,
                mockSecurityService.Object);
            //ACT 
            var actual = await sut.Handle(pagedQuery, CancellationToken.None);
            //ASSERT
            Assert.IsType<ResultListDto<AnotacionListItemDto>>(actual);
            mockIdentityService.Verify(t => t.GetUserIdentityInfo(), Times.Once);
            mockSecurityService.Verify(y => y.GetAccountByIdAsync(It.IsAny<string>(), null), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se encuentra la información de cuenta de seguridad por el id devuelve un objecto vacío")]
        public async Task Handle_ReturnInformacionCuentaSeguridadEmpty()
        {
            //ARRANGE 
            var mockIdentityService = new Mock<IIdentityService> { CallBase = true };
            var listItemDtos = new List<AnotacionListItemDto>();
            var itemDto = new AnotacionListItemDto
            {
                Id = 1,
                IdRefCuentaSeguridad = Guid.NewGuid().ToString()
            }; 
            listItemDtos.Add(itemDto);
             
            var mockSecurityService = new Mock<ISecurityService> { CallBase = true }; 
            mockSecurityService.Setup(x => x.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(null as AccountModel);

            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object,
                mockSecurityService.Object); 
            //ACT 
            await sut.GetInformacionCuentaSeguridad(listItemDtos);
            //ASSERT
            Assert.NotNull(itemDto.IdRefCuentaSeguridad);
            mockSecurityService.Verify(t => t.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }
        #endregion

        #region GetInformacionCuentaSeguridad

        [Fact(DisplayName = "Cuando se encuentra la cuenta de seguridad Devuelve Ok")]
        public async Task GetInformacionCuentaSeguridad_Ok()
        {
            //ARRANGE
            var anotaciones = new List<AnotacionListItemDto>
            {
                new (){ IdRefCuentaSeguridad = Guid.NewGuid().ToString()}
            };
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString()
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountInfo);
            var sut = new Mock<GetPagedAnotacionesQueryHandler>(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object)
            { CallBase = true };

            //ACT

            await sut.Object.GetInformacionCuentaSeguridad(anotaciones);

            //ASSERT
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            Assert.Equal($"{accountInfo.FirstName} {accountInfo.Surname}", anotaciones[0].NombreUsuario);
        }

        [Fact(DisplayName = "Cuando no se encuentra la cuenta de seguridad no Ejecuta método")]
        public async Task GetInformacionCuentaSeguridad_SinCuentaSeguridad()
        {
            //ARRANGE
            var anotaciones = new List<AnotacionListItemDto>();
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString()
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountInfo);
            var sut = new Mock<GetPagedAnotacionesQueryHandler>(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object)
            { CallBase = true };

            //ACT

            await sut.Object.GetInformacionCuentaSeguridad(anotaciones);

            //ASSERT
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ApplyQuery

        [Fact(DisplayName = "Cuando no se envía filtro Retorna todas las notas de un expediente")]
        public async Task ApplyQuery_Ok()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();
            await Context.Anotaciones.AddRangeAsync(Enumerable.Range(2, 10).Select(c => new Anotacion
            {
                Id = c,
                ExpedienteAlumno = expedienteAlumno,
                EsPublica = true
            }));
            await Context.SaveChangesAsync();
            var request = new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1
            };
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.Anotaciones.AsQueryable(), request, new AccountModel
            {
                Roles = new[] { AppConfiguration.KeyAdminRole }
            });

            //ASSERT
            Assert.NotEmpty(actual.ToList());
            Assert.Equal(10, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envía filtro de resumen Retorna todas las notas de un expediente que coincidan con el valor")]
        public async Task ApplyQuery_FiltroResumen()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();
            await Context.Anotaciones.AddRangeAsync(Enumerable.Range(2, 10).Select(c => new Anotacion
            {
                Id = c,
                ExpedienteAlumno = expedienteAlumno,
                EsPublica = true,
                Resumen = $"Resumen{c}"
            }));
            await Context.SaveChangesAsync();
            var request = new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1,
                Texto = "Resumen5"
            };
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.Anotaciones.AsQueryable(), request, new AccountModel
            {
                Roles = new[] { AppConfiguration.KeyAdminRole }
            });

            //ASSERT
            Assert.NotEmpty(actual.ToList());
            Assert.Single(actual.ToList());
        }

        [Fact(DisplayName = "Cuando se envía filtro de fecha desde Retorna todas las notas de un expediente que coincidan con el valor")]
        public async Task ApplyQuery_FiltroFechaDesde()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();
            await Context.Anotaciones.AddRangeAsync(Enumerable.Range(2, 10).Select(c => new Anotacion
            {
                Id = c,
                ExpedienteAlumno = expedienteAlumno,
                EsPublica = true,
                Resumen = $"Resumen{c}",
                Fecha = DateTime.Now.AddDays(c)
            }));
            await Context.SaveChangesAsync();
            var request = new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1,
                FechaDesde = DateTime.Now.AddDays(4)
            };
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.Anotaciones.AsQueryable(), request, new AccountModel
            {
                Roles = new[] { AppConfiguration.KeyAdminRole }
            });

            //ASSERT
            Assert.NotEmpty(actual.ToList());
            Assert.Equal(7, actual.ToList().Count);
        }

        [Fact(DisplayName = "Cuando se envía filtro de fecha hasta Retorna todas las notas de un expediente que coincidan con el valor")]
        public async Task ApplyQuery_FiltroFechaHasta()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            await Context.SaveChangesAsync();
            await Context.Anotaciones.AddRangeAsync(Enumerable.Range(2, 10).Select(c => new Anotacion
            {
                Id = c,
                ExpedienteAlumno = expedienteAlumno,
                EsPublica = true,
                Resumen = $"Resumen{c}",
                Fecha = DateTime.Now.AddDays(c)
            }));
            await Context.SaveChangesAsync();
            var request = new GetPagedAnotacionesQuery
            {
                IdExpedienteAlumno = 1,
                FechaHasta = DateTime.Now.AddDays(4)
            };
            var mockIdentityService = new Mock<IIdentityService>();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new GetPagedAnotacionesQueryHandler(Context, _mapper, mockIdentityService.Object, mockSecurityService.Object);

            //ACT
            var actual = sut.ApplyQuery(Context.Anotaciones.AsQueryable(), request, new AccountModel
            {
                Roles = new[] { AppConfiguration.KeyAdminRole }
            });

            //ASSERT
            Assert.NotEmpty(actual.ToList());
            Assert.Equal(3, actual.ToList().Count);
        }

        #endregion
    }
}
