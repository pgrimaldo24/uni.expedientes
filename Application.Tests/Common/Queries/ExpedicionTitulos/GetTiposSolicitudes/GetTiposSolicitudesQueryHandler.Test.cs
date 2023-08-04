using System;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes;
using Unir.Expedientes.Application.Common.Queries.GetRolesSecurity;
using Xunit;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Tests.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes
{
    [Collection("CommonTestCollection")]
    public class GetTiposSolicitudesQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetTiposSolicitudesQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle
        [Fact(DisplayName = "Cuando se obtiene tipo solicitud por id correctamente Retorna TipoSolicitudDto")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            AppConfiguration.KeyGestorRole = "gestor_expediente";
            var request = new GetTiposSolicitudesQuery { IdsRefUniversidad = new List<string> { "1", "3" } };
            var tiposSolicitudes = new List<TiposSolicitudesTitulosModel>
            {
                new ()
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    ConFechaPago = true,
                    RefCodigoTipoSolicitud = "SCAP"
                }
            };
            var mockITitulosService = new Mock<IExpedicionTitulosServiceClient> { CallBase = true };
            mockITitulosService.Setup(ts => ts.GetTiposSolicitudes(It.IsAny<TiposSolicitudesParameters>()))
                .ReturnsAsync(tiposSolicitudes);

            var mockIMediator = new Mock<IMediator> { CallBase = true };

            var roles = new[] { AppConfiguration.KeyAdminRole };
            mockIMediator.Setup(s => s.Send(It.IsAny<GetRolesSecurityQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetTiposSolicitudesQueryHandler>>
            {
                CallBase = true
            };

            var sut = new Mock<GetTiposSolicitudesQueryHandler>(mockITitulosService.Object, _mapper, mockIMediator.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var actual = await sut.Object.Handle(request, CancellationToken.None);


            //ASSERT
            Assert.Single(actual);
            Assert.Equal("SCAP", actual[0].RefCodigoTipoSolicitud);
            Assert.Equal(1, actual[0].Id);
            Assert.True(actual[0].ConFechaPago);
            mockITitulosService.Verify(ts => ts.GetTiposSolicitudes(It.IsAny<TiposSolicitudesParameters>()), Times.Once);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetRolesSecurityQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no tiene rol de admin y gestor Retorna BadRequest")]
        public async Task Handle_BadRequest()
        {
            //ARRANGE
            AppConfiguration.KeyAdminRole = "admin_expediente";
            AppConfiguration.KeyGestorRole = "gestor_expediente";
            string mensajeEsperado = $"El usuario no tiene contiene los roles " +
                                           $"{AppConfiguration.KeyAdminRole}, {AppConfiguration.KeyGestorRole}";
            var request = new GetTiposSolicitudesQuery { IdsRefUniversidad = new List<string> { "1", "3" } };
            var mockITitulosService = new Mock<IExpedicionTitulosServiceClient> { CallBase = true };
            var mockIMediator = new Mock<IMediator> { CallBase = true };
            var mockIStringLocalizer = new Mock<IStringLocalizer<GetTiposSolicitudesQueryHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new Mock<GetTiposSolicitudesQueryHandler>(mockITitulosService.Object, _mapper, mockIMediator.Object, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            var roles = new string[] { };
            mockIMediator.Setup(s => s.Send(It.IsAny<GetRolesSecurityQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(roles);

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockIMediator.Verify(s => s.Send(It.IsAny<GetRolesSecurityQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion
    }
}
