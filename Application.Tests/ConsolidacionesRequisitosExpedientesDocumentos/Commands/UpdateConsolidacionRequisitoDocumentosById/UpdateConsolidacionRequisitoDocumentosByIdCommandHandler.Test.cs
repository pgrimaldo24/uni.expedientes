using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UpdateConsolidacionRequisitoDocumentosById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientesDocumentos.Commands.UpdateConsolidacionRequisitoDocumentosById
{
    [Collection("CommonTestCollection")]
    public class UpdateConsolidacionRequisitoDocumentosByIdCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito documento Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new UpdateConsolidacionRequisitoDocumentosByIdCommand();
            var sut = new UpdateConsolidacionRequisitoDocumentosByIdCommandHandler(Context);

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
            var request = new UpdateConsolidacionRequisitoDocumentosByIdCommand
            {
                Id = 1,
                FicheroValidado = true
            };
            await Context.ConsolidacionesRequisitosExpedientesDocumentos.AddAsync(
                new ConsolidacionRequisitoExpedienteDocumento
                {
                    Id = 1,
                    FicheroValidado = false
                });
            await Context.SaveChangesAsync();
            var sut = new UpdateConsolidacionRequisitoDocumentosByIdCommandHandler(Context);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True((await Context.ConsolidacionesRequisitosExpedientesDocumentos.FirstAsync()).FicheroValidado);
        }

        #endregion
    }
}
