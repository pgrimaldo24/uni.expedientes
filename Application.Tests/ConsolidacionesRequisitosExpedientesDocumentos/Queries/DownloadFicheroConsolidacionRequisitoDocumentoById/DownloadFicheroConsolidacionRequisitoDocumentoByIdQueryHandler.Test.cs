using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.DownloadFicheroConsolidacionRequisitoDocumentoById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientesDocumentos.Queries.DownloadFicheroConsolidacionRequisitoDocumentoById
{
    [Collection("CommonTestCollection")]
    public class DownloadFicheroConsolidacionRequisitoDocumentoByIdQueryHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito documento Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery(1);
            var sut = new DownloadFicheroConsolidacionRequisitoDocumentoByIdQueryHandler(Context, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DownloadFicheroConsolidacionRequisitoDocumentoByIdQuery(1);
            await Context.ConsolidacionesRequisitosExpedientesDocumentos.AddAsync(
                new ConsolidacionRequisitoExpedienteDocumento
                {
                    Id = 1,
                    Fichero = "Alumno y Expedientes.png (Certificado servicio social)"
                });
            await Context.SaveChangesAsync();
            var mockIFileManager = new Mock<IFileManager> { CallBase = true };
            var sutMock = new Mock<DownloadFicheroConsolidacionRequisitoDocumentoByIdQueryHandler>(Context,
                    mockIFileManager.Object)
                { CallBase = true };
            mockIFileManager.Setup(x => x.ReadFileContentAsync(It.IsAny<string>()));

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockIFileManager.Verify(x => x.ReadFileContentAsync(It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
