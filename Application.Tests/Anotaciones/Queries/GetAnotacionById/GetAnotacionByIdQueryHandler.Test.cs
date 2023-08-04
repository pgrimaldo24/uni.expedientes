using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Anotaciones.Queries.GetAnotacionById
{
    [Collection("CommonTestCollection")]
    public class GetAnotacionByIdQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;

        public GetAnotacionByIdQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se encuentra una anotación devuelve Ok")]
        public async Task Handle_ExisteElemento_Ok()
        {
            //ARRANGE
            var id = 1;
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);
            var anotacion = new Anotacion
            {
                Id = 1,
                ExpedienteAlumno = expedienteAlumno
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new Mock<GetAnotacionByIdQueryHandler>(Context, _mapper, mockSecurityService.Object)
            { CallBase = true };

            //ACT
            var actual = await sut.Object.Handle(new GetAnotacionByIdQuery(id),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<AnotacionDto>(actual);
            Assert.Equal(Context.Anotaciones.FirstOrDefault()?.Id, anotacion.Id);
        }

        [Fact(DisplayName = "Cuando no se encuentra una anotación devuelve una excepción")]
        public async Task Handle_NoExisteElemento()
        {
            //ARRANGE
            var expedienteAlumno = new ExpedienteAlumno
            {
                Id = 1
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteAlumno);

            var anotacion = new Anotacion()
            {
                Id = 1,
                ExpedienteAlumno = expedienteAlumno
            };
            await Context.Anotaciones.AddAsync(anotacion);
            await Context.SaveChangesAsync();
            var mockSecurityService = new Mock<ISecurityService>();
            var sut = new Mock<GetAnotacionByIdQueryHandler>(Context, _mapper, mockSecurityService.Object)
            { CallBase = true };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(new GetAnotacionByIdQuery(99), CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("not found", ex.Message);
        }

        #endregion

        #region GetInformacionCuentaSeguridad

        [Fact(DisplayName = "Cuando se encuentra la cuenta de seguridad Devuelve Ok")]
        public async Task GetInformacionCuentaSeguridad_Ok()
        {
            //ARRANGE
            var anotacionDto = new AnotacionDto
            {
                IdRefCuentaSeguridad = Guid.NewGuid().ToString()
            };
            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString()
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountInfo);
             
            var sut = new Mock<GetAnotacionByIdQueryHandler>(Context, _mapper, mockSecurityService.Object)
            { CallBase = true };
             
            //ACT

            await sut.Object.GetInformacionCuentaSeguridad(anotacionDto); 

            //ASSERT
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
            Assert.Equal($"{accountInfo.FirstName} {accountInfo.Surname}", anotacionDto.NombreUsuario);
        }


        [Fact(DisplayName = "Cuando no se encuentra la cuenta de seguridad devuelve un valor vacío")]
        public async Task GetInformacionCuentaSeguridad_Empty()
        {
            //ARRANGE
            var anotacionDto = new AnotacionDto
            {
                IdRefCuentaSeguridad = Guid.NewGuid().ToString()
            };
            var mockIdentityService = new Mock<ISecurityService> { CallBase = true };

            mockIdentityService.Setup(x => x.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(null as AccountModel);
            var sut = new GetAnotacionByIdQueryHandler(Context, _mapper, mockIdentityService.Object);

            //ACT
            await sut.GetInformacionCuentaSeguridad(anotacionDto);

            //ASSERT
            Assert.Null(anotacionDto.NombreUsuario);
            mockIdentityService.Verify(vf => vf.GetAccountByIdAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once); 
        }


        [Fact(DisplayName = "Cuando no se encuentra la cuenta de seguridad no Ejecuta método")]
        public async Task GetInformacionCuentaSeguridad_SinCuentaSeguridad()
        {
            //ARRANGE
            var anotacionDto = new AnotacionDto();
            var mockSecurityService = new Mock<ISecurityService>();
            var accountInfo = new AccountModel
            {
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString()
            };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync(accountInfo);
            var sut = new Mock<GetAnotacionByIdQueryHandler>(Context, _mapper, mockSecurityService.Object)
            { CallBase = true };

            //ACT

            await sut.Object.GetInformacionCuentaSeguridad(anotacionDto);

            //ASSERT
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(), 
                It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
