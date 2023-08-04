using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateNivelUsoComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;
using System;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.CreateNivelUsoComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class CreateNivelUsoComportamientoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando el comportamiento no existe Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new CreateNivelUsoComportamientoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context,
                mockIMediator.Object, mockIStringLocalizer.Object);

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
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sutMock = new Mock<CreateNivelUsoComportamientoExpedienteCommandHandler>(Context,
                mockIMediator.Object, mockIStringLocalizer.Object) { CallBase = true };

            mockIMediator.Setup(s => s.Send(It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(), 
                    It.IsAny<CancellationToken>())).ReturnsAsync(new NivelUsoComportamientoExpediente());
            sutMock.Setup(x => x.ValidatePropiedades(
                It.IsAny<ICollection<NivelUsoComportamientoExpediente>>(),
                It.IsAny<CreateNivelUsoComportamientoExpedienteCommand>())).Returns(string.Empty);

            await Context.ComportamientosExpedientes.AddAsync(new ComportamientoExpediente
            {
                Id = 1
            });
            await Context.SaveChangesAsync();

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.True(actual > 0);
            mockIMediator.Verify(s => s.Send(It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<ICollection<NivelUsoComportamientoExpediente>>(),
                It.IsAny<CreateNivelUsoComportamientoExpedienteCommand>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para la Universidad Devuelve mensaje de error")]
        public void ValidatePropiedades_Universidad_ErrorMessage()
        {
            //ARRANGE
            var idRefUniversidad = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefUniversidad = idRefUniversidad,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.Universidad
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefUniversidad = idRefUniversidad,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.Universidad
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para la misma Universidad";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para el TipoEstudio Devuelve mensaje de error")]
        public void ValidatePropiedades_TipoEstudio_ErrorMessage()
        {
            //ARRANGE
            var idRefTipoEstudio = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefTipoEstudio = idRefTipoEstudio,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.TipoEstudio
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefTipoEstudio = idRefTipoEstudio,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.TipoEstudio
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para el mismo Tipo de Estudio";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para el Estudio Devuelve mensaje de error")]
        public void ValidatePropiedades_Estudio_ErrorMessage()
        {
            //ARRANGE
            var idRefEstudio = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefEstudio = idRefEstudio,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.Estudio
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefEstudio = idRefEstudio,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.Estudio
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para el mismo Estudio";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para el PlanEstudio Devuelve mensaje de error")]
        public void ValidatePropiedades_PlanEstudio_ErrorMessage()
        {
            //ARRANGE
            var idRefPlan = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefPlan = idRefPlan,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.PlanEstudio
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefPlan = idRefPlan,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.PlanEstudio
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para el mismo Plan";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para el TipoAsignatura Devuelve mensaje de error")]
        public void ValidatePropiedades_TipoAsignatura_ErrorMessage()
        {
            //ARRANGE
            var idRefTipoAsignatura = Guid.NewGuid().ToString();
            var idRefUniversidad = Guid.NewGuid().ToString();
            var idRefTipoEstudio = Guid.NewGuid().ToString();
            var idRefEstudio = Guid.NewGuid().ToString();
            var idRefPlan = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefTipoAsignatura = idRefTipoAsignatura,
                    IdRefUniversidad = idRefUniversidad,
                    IdRefTipoEstudio = idRefTipoEstudio,
                    IdRefEstudio = idRefEstudio,
                    IdRefPlan = idRefPlan,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.TipoAsignatura
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefTipoAsignatura = idRefTipoAsignatura,
                        IdRefUniversidad = idRefUniversidad,
                        IdRefTipoEstudio = idRefTipoEstudio,
                        IdRefEstudio = idRefEstudio,
                        IdRefPlan = idRefPlan,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.TipoAsignatura
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para el mismo Tipo de Asignatura";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        [Fact(DisplayName = "Cuando ya existe un Nivel Asociado del mismo Tipo para la AsignaturaPlan Devuelve mensaje de error")]
        public void ValidatePropiedades_AsignaturaPlan_ErrorMessage()
        {
            //ARRANGE
            var idRefAsignaturaPlan = Guid.NewGuid().ToString();
            var request = new CreateNivelUsoComportamientoExpedienteCommand
            {
                IdComportamiento = 1,
                NivelUsoComportamientoExpediente = new NivelUsoComportamientoExpedienteDto
                {
                    IdRefAsignaturaPlan = idRefAsignaturaPlan,
                    TipoNivelUso = new TipoNivelUsoDto
                    {
                        Id = TipoNivelUso.AsignaturaPlan
                    }
                }
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<CreateNivelUsoComportamientoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var sut = new CreateNivelUsoComportamientoExpedienteCommandHandler(Context, mockIMediator.Object, mockIStringLocalizer.Object);

            var comportamientoExpediente = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = new List<NivelUsoComportamientoExpediente>
                {
                    new()
                    {
                        Id = 1,
                        IdRefAsignaturaPlan = idRefAsignaturaPlan,
                        TipoNivelUso = new TipoNivelUso
                        {
                            Id = TipoNivelUso.AsignaturaPlan
                        }
                    },
                    new(),
                    new()
                }
            };
            var mensajeDeErrorEsperado = "Ya existe un Nivel Asociado del mismo Tipo para la misma Asignatura Plan";

            //ACT
            var actual = sut.ValidatePropiedades(comportamientoExpediente.NivelesUsoComportamientosExpedientes, request);

            //ASSERT
            Assert.Equal(actual, mensajeDeErrorEsperado);
        }

        #endregion
    }
}
