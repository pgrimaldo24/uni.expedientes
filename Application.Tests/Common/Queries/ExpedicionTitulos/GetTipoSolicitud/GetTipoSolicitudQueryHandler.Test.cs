using AutoMapper;
using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Common.Queries.ExpedicionTitulos.GetTipoSolicitud
{
    [Collection("CommonTestCollection")]
    public class GetTipoSolicitudQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;
        public GetTipoSolicitudQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle
        [Fact(DisplayName = "Cuando se obtiene tipo solicitud por id correctamente Retorna TipoSolicitudDto")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var mockTitulosService = new Mock<IExpedicionTitulosServiceClient> { CallBase = true };

            var tipoSolicitud = new TipoSolicitudTituloModel
            {
                Id = 1,
                Nombre = "CERTIFICADO",
                IdRefUniversidad = "1",
                ConFechaPago = true,
                RefCodigoTipoSolicitud = "CEC"
            };
            mockTitulosService.Setup(ts => ts.GetTipoSolicitudTituloById(It.IsAny<int>()))
                .ReturnsAsync(tipoSolicitud);

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetTipoSolicitudQueryHandler>>
            {
                CallBase = true
            };

            var sut = new Mock<GetTipoSolicitudQueryHandler>(mockTitulosService.Object, _mapper, mockIStringLocalizer.Object)
            { CallBase = true };

            //ACT
            var actual = await sut.Object.Handle(new GetTipoSolicitudQuery(1), CancellationToken.None);

            //ASSERT
            Assert.IsType<TipoSolicitudDto>(actual);
            Assert.Equal("CERTIFICADO", actual.Nombre);
            Assert.Equal("1", actual.IdRefUniversidad);
            Assert.True(actual.ConFechaPago);
            Assert.Equal("CEC", actual.RefCodigoTipoSolicitud);
            mockTitulosService.Verify(ts => ts.GetTipoSolicitudTituloById(It.IsAny<int>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando el tipo solicitud no se encuentra Retorna BadRequest")]
        public async Task Handle_BadRequest()
        {
            //ARRANGE
            const string mensajeEsperado = "El tipo de solicitud de título no ha sido encontrado";
            var request = new GetTipoSolicitudQuery(1);

            var mockTitulosService = new Mock<IExpedicionTitulosServiceClient> { CallBase = true };

            mockTitulosService.Setup(ts => ts.GetTipoSolicitudTituloById(It.IsAny<int>()))
                .ReturnsAsync((TipoSolicitudTituloModel)null);

            var mockIStringLocalizer = new Mock<IStringLocalizer<GetTipoSolicitudQueryHandler>>
            {
                CallBase = true
            };
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var sut = new Mock<GetTipoSolicitudQueryHandler>(mockTitulosService.Object, _mapper, mockIStringLocalizer.Object)
            { CallBase = true };

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockTitulosService.Verify(ts => ts.GetTipoSolicitudTituloById(It.IsAny<int>()),Times.Once);
        }
        #endregion
    }
}
