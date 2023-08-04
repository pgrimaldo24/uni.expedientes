using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisito;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Commands.DeleteRequisito
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el requisito no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new DeleteRequisitoCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new Mock<DeleteRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
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
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando el requisito tiene consolidaciones en los expedientes Devuelve una excepción")]
        public async Task Handle_consolidaciones_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitoCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "El requisito tiene consolidaciones en los expedientes.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<DeleteRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando el requisito tiene comportamiento Devuelve una excepción")]
        public async Task Handle_Comportamiento_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitoCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "El requisito es usado por algún comportamiento.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<DeleteRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();

            //ACT
            var ex = (BadRequestException)await Record.ExceptionAsync(async () =>
            {
                await sut.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando se elimina el requisito Devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitoCommand(1);
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteRequisitoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.DeleteDependencias(It.IsAny<RequisitoExpediente>()));

            var requisito = new RequisitoExpediente
            {
                Id = 1
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(await Context.RequisitosExpedientes.FirstOrDefaultAsync(r => r.Id == 1));
            sutMock.Verify(x => x.DeleteDependencias(It.IsAny<RequisitoExpediente>()), Times.Once);
        }
        #endregion

        #region DeleteDependencias

        [Fact(DisplayName = "Cuando se eliminan las dependencias del requisito Devuelve Ok")]
        public async Task DeleteDependencias_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitoCommandHandler(Context, mockIStringLocalizer.Object);

            var requisito = new RequisitoExpediente
            {
                Id = 1,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = 1
                },
                CausasEstadosRequisitosConsolidadasExpediente = new List<CausaEstadoRequisitoConsolidadaExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                },
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>
                {
                    new()
                    {
                        Id = 1
                    }
                },
                RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>
                {
                    new()
                    {
                        Id = 1,
                        ConsolidacionesRequisitosExpedientesDocumentos = new List<ConsolidacionRequisitoExpedienteDocumento>
                        {
                            new()
                            {
                                Id = 1
                            }
                        }
                    }
                },
                RequisitosExpedientesFilesType = new List<RequisitoExpedienteFileType>
                {
                    new()
                    {
                        Id = 1
                    }
                },
                RolesRequisitosExpedientes = new List<RolRequisitoExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.SaveChangesAsync();

            //ACT
            sut.DeleteDependencias(requisito);

            //ASSERT
            Assert.Empty(requisito.CausasEstadosRequisitosConsolidadasExpediente);
            Assert.Empty(requisito.RequisitosExpedientesRequerimientosTitulos);
            Assert.Empty(requisito.RequisitosExpedientesDocumentos);
            Assert.Empty(requisito.RequisitosExpedientesFilesType);
            Assert.Empty(requisito.RolesRequisitosExpedientes);
        }

        #endregion
    }
}
