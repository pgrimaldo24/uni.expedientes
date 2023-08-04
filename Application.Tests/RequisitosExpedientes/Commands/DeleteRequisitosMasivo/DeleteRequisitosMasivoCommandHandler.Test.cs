using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisitosMasivo;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.RequisitosExpedientes.Commands.DeleteRequisitosMasivo
{
    [Collection("CommonTestCollection")]
    public class DeleteRequisitosMasivoCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando los requisitos no existen Devuelve una excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var request = new DeleteRequisitosMasivoCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosMasivoCommandHandler>>
            {
                CallBase = true
            };
            const string mensajeEsperado = "Los requisitos seleccionados no existen.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));
            var sut = new Mock<DeleteRequisitosMasivoCommandHandler>(Context, mockIStringLocalizer.Object)
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

        [Fact(DisplayName = "Cuando se eliminan los requisitos pero hay algunos que tienen relación con otras tablas Devuelve Ok con mensajes")]
        public async Task Handle_Message_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitosMasivoCommand
            {
                IdsRequisitos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteRequisitosMasivoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.DeleteDependencias(It.IsAny<List<RequisitoExpediente>>()));
            var requisito = new RequisitoExpediente
            {
                Id = 1
            };
            var requisitoConsolidacion = new RequisitoExpediente
            {
                Id = 2,
                ConsolidacionesRequisitosExpedientes = new List<ConsolidacionRequisitoExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            var requisitoComportamiento = new RequisitoExpediente
            {
                Id = 3,
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1
                    }
                }
            };
            await Context.RequisitosExpedientes.AddAsync(requisito);
            await Context.RequisitosExpedientes.AddAsync(requisitoConsolidacion);
            await Context.RequisitosExpedientes.AddAsync(requisitoComportamiento);
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotEmpty(actual);
            Assert.Equal(2, actual.Count);
            sutMock.Verify(x => x.DeleteDependencias(It.IsAny<List<RequisitoExpediente>>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se eliminan los requisitos pero no hay relación con otras tablas Devuelve Ok sin mensajes")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var request = new DeleteRequisitosMasivoCommand
            {
                IdsRequisitos = new List<int>
                {
                    1, 2, 3
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new Mock<DeleteRequisitosMasivoCommandHandler>(Context, mockIStringLocalizer.Object)
            {
                CallBase = true
            };
            sutMock.Setup(x => x.DeleteDependencias(It.IsAny<List<RequisitoExpediente>>()));
            await Context.RequisitosExpedientes.AddRangeAsync(Enumerable.Range(1, 3).Select(r => new RequisitoExpediente
            {
                Id = r
            }));
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Empty(actual);
            Assert.Empty(await Context.RequisitosExpedientes.ToListAsync());
            sutMock.Verify(x => x.DeleteDependencias(It.IsAny<List<RequisitoExpediente>>()), Times.Once);
        }

        #endregion

        #region DeleteDependencias

        [Fact(DisplayName = "Cuando se eliminan las dependencias del requisito Devuelve Ok")]
        public async Task DeleteDependencias_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<DeleteRequisitosMasivoCommandHandler>>
            {
                CallBase = true
            };
            var sut = new DeleteRequisitosMasivoCommandHandler(Context, mockIStringLocalizer.Object);
            var requisitos = Enumerable.Range(1, 3).Select(x => new RequisitoExpediente
            {
                Id = x,
                EstadoExpediente = new EstadoExpediente
                {
                    Id = x
                },
                CausasEstadosRequisitosConsolidadasExpediente = new List<CausaEstadoRequisitoConsolidadaExpediente>
                {
                    new()
                    {
                        Id = x
                    }
                },
                RequisitosExpedientesRequerimientosTitulos = new List<RequisitoExpedienteRequerimientoTitulo>
                {
                    new()
                    {
                        Id = x
                    }
                },
                RequisitosExpedientesDocumentos = new List<RequisitoExpedienteDocumento>
                {
                    new()
                    {
                        Id = x,
                        ConsolidacionesRequisitosExpedientesDocumentos =
                            new List<ConsolidacionRequisitoExpedienteDocumento>
                            {
                                new()
                                {
                                    Id = x
                                }
                            }
                    }
                },
                RequisitosExpedientesFilesType = new List<RequisitoExpedienteFileType>
                {
                    new()
                    {
                        Id = x
                    }
                },
                RolesRequisitosExpedientes = new List<RolRequisitoExpediente>
                {
                    new()
                    {
                        Id = x
                    }
                }
            }).ToList();
            await Context.RequisitosExpedientes.AddRangeAsync(requisitos);
            await Context.SaveChangesAsync();

            //ACT
            sut.DeleteDependencias(requisitos);

            //ASSERT
            Assert.Empty(requisitos.SelectMany(r => r.CausasEstadosRequisitosConsolidadasExpediente));
            Assert.Empty(requisitos.SelectMany(r => r.RequisitosExpedientesRequerimientosTitulos));
            Assert.Empty(requisitos.SelectMany(r => r.RequisitosExpedientesDocumentos));
            Assert.Empty(requisitos.SelectMany(r => r.RequisitosExpedientesFilesType));
            Assert.Empty(requisitos.SelectMany(r => r.RolesRequisitosExpedientes));
        }

        #endregion
    }
}
