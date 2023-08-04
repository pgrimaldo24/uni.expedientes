using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.DeleteConsolidacionRequisitoDocumentosById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientesDocumentos.Commands.DeleteConsolidacionRequisitoDocumentosById
{
    [Collection("CommonTestCollection")]
    public class DeleteConsolidacionRequisitoDocumentosByIdCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito documento Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteConsolidacionRequisitoDocumentosByIdCommand(1);
            var sut = new DeleteConsolidacionRequisitoDocumentosByIdCommandHandler(Context, null);

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
            var request = new DeleteConsolidacionRequisitoDocumentosByIdCommand(1);
            await Context.ConsolidacionesRequisitosExpedientesDocumentos.AddAsync(
                new ConsolidacionRequisitoExpedienteDocumento
                {
                    Id = 1,
                    Fichero = "Alumno y Expedientes.png (Certificado servicio social)"
                });
            await Context.SaveChangesAsync();
            var mockIFileManager = new Mock<IFileManager> { CallBase = true };
            var sutMock = new Mock<DeleteConsolidacionRequisitoDocumentosByIdCommandHandler>(Context, 
                mockIFileManager.Object) { CallBase = true };
            mockIFileManager.Setup(x => x.RemoveFile(It.IsAny<string>()));

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.False(await Context.ConsolidacionesRequisitosExpedientesDocumentos.AnyAsync());
            mockIFileManager.Verify(x => x.RemoveFile(It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
