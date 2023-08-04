using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.DeleteRequisitosExpedientesDocumentos;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientesDocumentos.Commands.DeleteRequisitosExpedientesDocumentos
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitosExpedientesDocumentosCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los requisitos no existen Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitosExpedientesDocumentosCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosExpedientesDocumentosCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los documentos seleccionados no existen.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<DeleteRequisitosExpedientesDocumentosCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
        }

        [Fact(DisplayName = "Cuando se eliminan lo documentos requisitos pero hay algunos que tienen relación con otras tablas Devuelve Ok con mensajes")]
        public async Task Handle_Message_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitosExpedientesDocumentosCommand
            {
                IdsDocumentos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosExpedientesDocumentosCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteRequisitosExpedientesDocumentosCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            var requisitoDocumento = new RequisitoExpedienteDocumento
            {
                Id = 1
            };
            var requisitoDocumentoConsolidacion = new RequisitoExpedienteDocumento
            {
                Id = 2,
                ConsolidacionesRequisitosExpedientesDocumentos = new List<ConsolidacionRequisitoExpedienteDocumento>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoDocumento);
            await Context.RequisitosExpedientesDocumentos.AddAsync(requisitoDocumentoConsolidacion);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(1, actual.Count);
        }

        [Fact(DisplayName = "Cuando se eliminan los documentos requisitos pero no hay relación con otras tablas Devuelve Ok sin mensajes")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitosExpedientesDocumentosCommand
            {
                IdsDocumentos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosExpedientesDocumentosCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteRequisitosExpedientesDocumentosCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            await Context.RequisitosExpedientesDocumentos.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpedienteDocumento
            {
                Id = r
            }));
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
            Assert.Empty(await Context.RequisitosExpedientesDocumentos.ToListAsync());
        }

        #endregion
    }
}
