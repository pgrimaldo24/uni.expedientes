using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Moq;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitoExpedienteDocumento;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientesDocumentos.Commands.DeleteRequisitoExpedienteDocumento
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitoExpedienteDocumentoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito documento no existe Devuelve una excepción")]
        public async Task Handle_DocumentoInexistente()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var request = new DeleteRequisitoExpedienteDocumentoCommand(1);
            var sut = new DeleteRequisitoExpedienteDocumentoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(ex);
            Assert.Contains("not found", ex.Message);
        }

        [Fact(DisplayName = "Cuando el requisito documento tiene consolidaciones Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "El documento tiene asociado una consolidación.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var request = new DeleteRequisitoExpedienteDocumentoCommand(1);
            var sut = new DeleteRequisitoExpedienteDocumentoCommandHandler(Context, mockIStringLocalizer.Object);
            var requisitoExpedienteDocumento = new RequisitoExpedienteDocumento
            {
                Id = 1,
                ConsolidacionesRequisitosExpedientesDocumentos = new List<ConsolidacionRequisitoExpedienteDocumento>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento);
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando elimina un requisito documento Devuelve Ok")]
        public async Task Handle_EliminaDocumento_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoExpedienteDocumentoCommandHandler>>
            {
                CallBase = true
            };
            var requisitoExpedienteDocumento = new RequisitoExpedienteDocumento
            {
                Id = 1
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoExpedienteDocumento);
            await Context.SaveChangesAsync();

            var request = new DeleteRequisitoExpedienteDocumentoCommand(1);
            var sut = new DeleteRequisitoExpedienteDocumentoCommandHandler(Context, mockIStringLocalizer.Object);

            //ACT
            await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(Context.RequisitosExpedientesDocumentos.FirstOrDefault());
        }

        #endregion
    }
}
