using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Queries.GetRolesSecurity;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Common.Queries.GetRolesSecurity
{
    [Collection("CommonTestCollection")]
    public class GetRolesSecurityQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando obtiene roles de la cuenta de usuario correctamente Retorna roles")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            AppConfiguration.KeyGestorRole = "gestor_expediente";
            var request = new GetRolesSecurityQuery();

            var mockIdentityService = new Mock<IIdentityService> { CallBase = true };
            var mockSecurityService = new Mock<ISecurityService> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetRolesSecurityQueryHandler>>
            {
                CallBase = true
            };

            var identityModel = new IdentityModel { Id = "1", Roles = new[] { AppConfiguration.KeyAdminRole, AppConfiguration.KeyGestorRole } };
            mockIdentityService.Setup(i => i.GetUserIdentityInfo())
                .Returns(identityModel);

            var accountModel = new AccountModel();
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(accountModel);


            var sut = new Mock<GetRolesSecurityQueryHandler>(mockIdentityService.Object, mockSecurityService.Object, mockIStringLocalizer.Object)
            { CallBase = true };

            //ACT 
            var actual = await sut.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(2, actual.Length);
            Assert.Equal(AppConfiguration.KeyAdminRole, actual[0]);
            Assert.Equal(AppConfiguration.KeyGestorRole, actual[1]);
            mockIdentityService.Verify(i => i.GetUserIdentityInfo(), Times.Once);
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no obtiene identificación del usuario Retorna BadRequest")]
        public async Task Handle_BadRequest_Identity()
        {
            //ARRANGE
            const string mensajeEsperado = "La identidad de usuario no existe";

            var mockIdentityService = new Mock<IIdentityService> { CallBase = true };
            mockIdentityService.Setup(i => i.GetUserIdentityInfo())
                .Returns((IdentityModel)null);

            var mockSecurityService = new Mock<ISecurityService> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetRolesSecurityQueryHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new Mock<GetRolesSecurityQueryHandler>(mockIdentityService.Object, mockSecurityService.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(new GetRolesSecurityQuery(), CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIdentityService.Verify(i => i.GetUserIdentityInfo(), Times.Once);
        }

        [Fact(DisplayName = "Cuando no obtiene la cuenta de usuario Retorna BadRequest")]
        public async Task Handle_BadRequest_Account()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            AppConfiguration.KeyGestorRole = "gestor_expediente";
            const string mensajeEsperado = "La cuenta de usuario no existe";

            var identityModel = new IdentityModel { Id = "1", Roles = new[] { AppConfiguration.KeyAdminRole, AppConfiguration.KeyGestorRole } };
            var mockIdentityService = new Mock<IIdentityService> { CallBase = true };
            mockIdentityService.Setup(i => i.GetUserIdentityInfo())
                .Returns(identityModel);

            var mockSecurityService = new Mock<ISecurityService> { CallBase = true };
            mockSecurityService.Setup(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((AccountModel)null);

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetRolesSecurityQueryHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new Mock<GetRolesSecurityQueryHandler>(mockIdentityService.Object, mockSecurityService.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(new GetRolesSecurityQuery(), CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIdentityService.Verify(i => i.GetUserIdentityInfo(), Times.Once);
            mockSecurityService.Verify(s => s.GetAccountByIdAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
