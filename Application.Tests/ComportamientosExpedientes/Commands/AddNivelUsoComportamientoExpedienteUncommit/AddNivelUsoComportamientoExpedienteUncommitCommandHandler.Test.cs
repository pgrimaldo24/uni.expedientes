using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit
{
    [Collection("CommonTestCollection")]
    public class AddNivelUsoComportamientoExpedienteUncommitCommandHandlerTests : TestBase
    {

        #region Handle

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso universidad Devuelve un objeto")]
        public async Task Handle_Universidad_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.Universidad,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetUniversidadNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Universidad
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetUniversidadNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso tipo estudio Devuelve un objeto")]
        public async Task Handle_TipoEstudio_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.TipoEstudio,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetTipoEstudioNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoEstudio
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, actual.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetTipoEstudioNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso estudio Devuelve un objeto")]
        public async Task Handle_Estudio_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.Estudio,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetEstudioNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Estudio
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, actual.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefEstudio, actual.IdRefEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreEstudio, actual.NombreEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetEstudioNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso plan estudio Devuelve un objeto")]
        public async Task Handle_PlanEstudio_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.PlanEstudio,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetPlanEstudioNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.PlanEstudio
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, actual.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefEstudio, actual.IdRefEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreEstudio, actual.NombreEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(nivelUsoComportamientoExpediente.NombrePlan, actual.NombrePlan);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetPlanEstudioNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso tipo asignatura Devuelve un objeto")]
        public async Task Handle_TipoAsignatura_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = Guid.NewGuid().ToString(),
                NombreTipoAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.TipoAsignatura,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetTipoAsignaturaNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoAsignatura
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, actual.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefEstudio, actual.IdRefEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreEstudio, actual.NombreEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(nivelUsoComportamientoExpediente.NombrePlan, actual.NombrePlan);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoAsignatura, actual.IdRefTipoAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoAsignatura, actual.NombreTipoAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetTipoAsignaturaNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos del nivel de uso asignatura Devuelve un objeto")]
        public async Task Handle_AsignaturaPlan_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = Guid.NewGuid().ToString(),
                NombreTipoAsignatura = Guid.NewGuid().ToString(),
                IdRefAsignaturaPlan = Guid.NewGuid().ToString(),
                IdRefAsignatura = Guid.NewGuid().ToString(),
                NombreAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.AsignaturaPlan,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            sutMock.Setup(x => x.GetAsignaturaPlanNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.AsignaturaPlan
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var actual = await sutMock.Object.Handle(
                request, CancellationToken.None);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, actual.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefEstudio, actual.IdRefEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreEstudio, actual.NombreEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(nivelUsoComportamientoExpediente.NombrePlan, actual.NombrePlan);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoAsignatura, actual.IdRefTipoAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoAsignatura, actual.NombreTipoAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefAsignaturaPlan, actual.IdRefAsignaturaPlan);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefAsignatura, actual.IdRefAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreAsignatura, actual.NombreAsignatura);
            Assert.Equal(nivelUsoComportamientoExpediente.TipoNivelUso.Id, actual.TipoNivelUso.Id);
            sutMock.Verify(x => x.GetAsignaturaPlanNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se asignan los datos y no existe el tipo nivel de uso Devuelve excepción")]
        public async Task Handle_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            const string mensajeEsperado = "Debe seleccionar un Tipo de Nivel de Uso.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));


            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente
            {
                TipoNivelUso = new TipoNivelUso
                {
                    Id = TipoNivelUso.Universidad,
                    Nombre = Guid.NewGuid().ToString()
                }
            };

            await Context.TiposNivelesUso.AddAsync(nivelUsoComportamientoExpediente.TipoNivelUso);
            await Context.SaveChangesAsync();

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = 100
                }
            };
            var request = new AddNivelUsoComportamientoExpedienteUncommitCommand(nivelUsoComportamiento);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region GetUniversidadNivelDeUso

        [Fact(DisplayName = "Cuando se obtiene la universidad Devuelve un objeto")]
        public async Task GetUniversidadNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var universidad = new UniversidadAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Acronimo = Guid.NewGuid().ToString()
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetUniversidadById(It.IsAny<int>()))
                .ReturnsAsync(universidad);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.Universidad
            };

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetUniversidadNivelDeUso(
                nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(universidad.Acronimo, actual.AcronimoUniversidad);
            Assert.Equal(universidad.Id.ToString(), actual.IdRefUniversidad);
            Assert.Equal(tipoNivelUso.Id, actual.TipoNivelUso.Id);
            mockIErpAcademicoServiceClient.Verify(x => x.GetUniversidadById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetTipoEstudioNivelDeUso

        [Fact(DisplayName = "Cuando se obtiene el tipo de estudio Devuelve un objeto")]
        public async Task GetTipoEstudioNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
                { CallBase = true };

            var tipoEstudio = new TipoEstudioAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Universidad = new UniversidadAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    Acronimo = Guid.NewGuid().ToString()
                }
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoEstudioById(It.IsAny<int>()))
                .ReturnsAsync(tipoEstudio);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.TipoEstudio
            };

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefTipoEstudio = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetTipoEstudioNivelDeUso(
                nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(tipoEstudio.Universidad.Acronimo, actual.AcronimoUniversidad);
            Assert.Equal(tipoEstudio.Universidad.Id.ToString(), actual.IdRefUniversidad);
            Assert.Equal(tipoEstudio.Id.ToString(), actual.IdRefTipoEstudio);
            Assert.Equal(tipoEstudio.Nombre, actual.NombreTipoEstudio);
            Assert.Equal(tipoNivelUso.Id, actual.TipoNivelUso.Id);
            mockIErpAcademicoServiceClient.Verify(x => x.GetTipoEstudioById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetEstudioNivelDeUso

        [Fact(DisplayName = "Cuando se obtiene el estudio Devuelve un objeto")]
        public async Task GetEstudioNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var estudio = new EstudioAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Tipo = new TipoEstudioAcademicoModel
                {
                    Id = 1
                }
            };
            var tipoEstudio = new TipoEstudioAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Universidad = new UniversidadAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    Acronimo = Guid.NewGuid().ToString()
                }
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetEstudioById(It.IsAny<int>()))
                .ReturnsAsync(estudio);
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoEstudioById(It.IsAny<int>()))
                .ReturnsAsync(tipoEstudio);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.Estudio
            };

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefEstudio = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetEstudioNivelDeUso(
                nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(tipoEstudio.Universidad.Acronimo, actual.AcronimoUniversidad);
            Assert.Equal(tipoEstudio.Universidad.Id.ToString(), actual.IdRefUniversidad);
            Assert.Equal(tipoEstudio.Id.ToString(), actual.IdRefTipoEstudio);
            Assert.Equal(tipoEstudio.Nombre, actual.NombreTipoEstudio);
            Assert.Equal(estudio.Id.ToString(), actual.IdRefEstudio);
            Assert.Equal(estudio.Nombre, actual.NombreEstudio);
            Assert.Equal(tipoNivelUso.Id, actual.TipoNivelUso.Id);
            mockIErpAcademicoServiceClient.Verify(x => x.GetEstudioById(It.IsAny<int>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetTipoEstudioById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetPlanEstudioNivelDeUso

        [Fact(DisplayName = "Cuando se obtiene el plan de estudio Devuelve un objeto")]
        public async Task GetPlanEstudioNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var planEstudio = new PlanAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Estudio = new EstudioAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    Tipo = new TipoEstudioAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                    },
                    AreaAcademica = new AreaAcademicaAcademicoModel
                    {
                        Centro = new CentroAcademicoModel
                        {
                            Universidad = new UniversidadAcademicoModel
                            {
                                Id = 1,
                                Acronimo = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetPlanEstudioById(It.IsAny<int>()))
                .ReturnsAsync(planEstudio);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.PlanEstudio
            };

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefPlan = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetPlanEstudioNivelDeUso(
                nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(planEstudio.Estudio.AreaAcademica.Centro.Universidad.Acronimo, actual.AcronimoUniversidad);
            Assert.Equal(planEstudio.Estudio.AreaAcademica.Centro.Universidad.Id.ToString(), actual.IdRefUniversidad);
            Assert.Equal(planEstudio.Estudio.Tipo.Id.ToString(), actual.IdRefTipoEstudio);
            Assert.Equal(planEstudio.Estudio.Tipo.Nombre, actual.NombreTipoEstudio);
            Assert.Equal(planEstudio.Estudio.Id.ToString(), actual.IdRefEstudio);
            Assert.Equal(planEstudio.Estudio.Nombre, actual.NombreEstudio);
            Assert.Equal(planEstudio.Id.ToString(), actual.IdRefPlan);
            Assert.Equal(planEstudio.Nombre, actual.NombrePlan);
            Assert.Equal(tipoNivelUso.Id, actual.TipoNivelUso.Id);
            mockIErpAcademicoServiceClient.Verify(x => x.GetPlanEstudioById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetTipoAsignaturaNivelDeUso

        [Fact(DisplayName = "Cuando se validan las propiedades del tipo de asignatura Devuelve una excepción")]
        public async Task GetTipoAsignaturaNivelDeUso_ValidatePropiedadesRequeridasNivelTipoAsignatura_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object) { CallBase = true };
            const string mensajeEsperado = "Mensaje de error.";
            sutMock.Setup(x => x.ValidatePropiedadesRequeridasNivelTipoAsignatura(
                It.IsAny<NivelUsoComportamientoExpedienteDto>())).Throws(new BadRequestException(mensajeEsperado));

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto();
            var tipoNivelUso = new TipoNivelUso();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.Object.GetTipoAsignaturaNivelDeUso(nivelUsoComportamiento, tipoNivelUso);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            sutMock.Verify(x => x.ValidatePropiedadesRequeridasNivelTipoAsignatura(
                It.IsAny<NivelUsoComportamientoExpedienteDto>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve un objeto")]
        public async Task GetTipoAsignaturaNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
                { CallBase = true };
           
            sutMock.Setup(x => x.ValidatePropiedadesRequeridasNivelTipoAsignatura(
                It.IsAny<NivelUsoComportamientoExpedienteDto>()));

            var nivelUsoUniversidad = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString()
            };
            sutMock.Setup(x => x.GetUniversidadNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoUniversidad);

            var tipoAsignatura = new TipoAsignaturaErpAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString()
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoAsignaturaById(It.IsAny<int>()))
                .ReturnsAsync(tipoAsignatura);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.TipoAsignatura
            };
            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefTipoAsignatura = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetTipoAsignaturaNivelDeUso(nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(tipoAsignatura.Id.ToString(), actual.IdRefTipoAsignatura);
            Assert.Equal(tipoAsignatura.Nombre, actual.NombreTipoAsignatura);
            Assert.Equal(nivelUsoUniversidad.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoUniversidad.IdRefUniversidad, actual.IdRefUniversidad);
            sutMock.Verify(x => x.ValidatePropiedadesRequeridasNivelTipoAsignatura(
                It.IsAny<NivelUsoComportamientoExpedienteDto>()), Times.Once());
            mockIErpAcademicoServiceClient.Verify(x => x.GetTipoAsignaturaById(It.IsAny<int>()), Times.Once());
            sutMock.Verify(x => x.GetUniversidadNivelDeUso(
                It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()), Times.Once());
        }

        [Fact(DisplayName = "Cuando el proceso es correcto Devuelve un objeto")]
        public async Task GetTipoAsignaturaNivelDeUsoNotNull()
        {
            //ARRANGE 
            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefTipoAsignatura = "1",
            };
            var nivelusoDto = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUso
                {
                    Nombre = Guid.NewGuid().ToString(),
                    Orden = 1
                }
            };
             
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
                { CallBase = true };

            
            sutMock.Setup(x => x.ValidatePropiedadesRequeridasNivelTipoAsignatura(
                It.IsAny<NivelUsoComportamientoExpedienteDto>()));
             
            var nivelUsoUniversidad = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString()
            };
            sutMock.Setup(x => x.GetUniversidadNivelDeUso(
                    It.IsAny<NivelUsoComportamientoExpedienteDto>(), It.IsAny<TipoNivelUso>()))
                .ReturnsAsync(nivelUsoUniversidad);

            var tipoAsignatura = new TipoAsignaturaErpAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString()
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetTipoAsignaturaById(It.IsAny<int>()))
                .ReturnsAsync(tipoAsignatura);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.TipoAsignatura
            };

            var nivelUso = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
            };
            //ACT
            var actual = await sutMock.Object.GetTipoAsignaturaNivelDeUso(nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.NotNull(nivelusoDto);
            Assert.NotNull(nivelUsoComportamiento);
            Assert.NotNull(nivelUso);
            Assert.Equal(tipoAsignatura.Id.ToString(), actual.IdRefTipoAsignatura);
            Assert.Equal(tipoAsignatura.Nombre, actual.NombreTipoAsignatura);
            Assert.Equal(nivelUsoUniversidad.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(nivelUsoUniversidad.IdRefUniversidad, actual.IdRefUniversidad);
        }
        #endregion

        #region ValidatePropiedadesRequeridasNivelTipoAsignatura

        [Fact(DisplayName = "Cuando no se envía un tipo de asignatura Devuelve una excepción")]
        public void ValidatePropiedadesRequeridasNivelTipoAsignatura_IdRefTipoAsignatura_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new AddNivelUsoComportamientoExpedienteUncommitCommandHandler(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object);
            const string mensajeEsperado = "Debe seleccionar un Tipo de Asignatura.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto();

            //ACT
            var ex = Record.Exception( () =>
            {
                sut.ValidatePropiedadesRequeridasNivelTipoAsignatura(nivelUsoComportamiento);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        [Fact(DisplayName = "Cuando no se envía la universidad Devuelve una excepción")]
        public void ValidatePropiedadesRequeridasNivelTipoAsignatura_IdRefUniversidad_BadRequestException()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new AddNivelUsoComportamientoExpedienteUncommitCommandHandler(Context, mockIStringLocalizer.Object,
                mockIErpAcademicoServiceClient.Object);
            const string mensajeEsperado = "Debe seleccionar una Universidad.";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefTipoAsignatura = "1"
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.ValidatePropiedadesRequeridasNivelTipoAsignatura(nivelUsoComportamiento);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
        }

        #endregion

        #region AssignNivelUsoTipoAsignatura

        [Fact(DisplayName = "Cuando se asignan los datos al nivel de uso tipo asignatura Devuelve un objeto")]
        public void AssignNivelUsoTipoAsignatura_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sut = new AddNivelUsoComportamientoExpedienteUncommitCommandHandler(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object);

            var nivelUso = new NivelUsoComportamientoExpediente
            {
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
            };
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente();

            //ACT
            sut.AssignNivelUsoTipoAsignatura(nivelUso, nivelUsoComportamientoExpediente);

            //ASSERT
            Assert.Equal(nivelUsoComportamientoExpediente.AcronimoUniversidad, nivelUso.AcronimoUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefUniversidad, nivelUso.IdRefUniversidad);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefTipoEstudio, nivelUso.IdRefTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreTipoEstudio, nivelUso.NombreTipoEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefEstudio, nivelUso.IdRefEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.NombreEstudio, nivelUso.NombreEstudio);
            Assert.Equal(nivelUsoComportamientoExpediente.IdRefPlan, nivelUso.IdRefPlan);
            Assert.Equal(nivelUsoComportamientoExpediente.NombrePlan, nivelUso.NombrePlan);
        } 

        #endregion

        #region GetAsignaturaNivelDeUso

        [Fact(DisplayName = "Cuando se obtiene la asignatura Devuelve un objeto")]
        public async Task GetAsignaturaNivelDeUso_Ok()
        {
            //ARRANGE
            var mockIStringLocalizer = new Mock<IStringLocalizer<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>>
            {
                CallBase = true
            };
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<AddNivelUsoComportamientoExpedienteUncommitCommandHandler>(Context, mockIStringLocalizer.Object,
                    mockIErpAcademicoServiceClient.Object)
            { CallBase = true };

            var asignatura = new AsignaturaPlanErpAcademicoModel
            {
                Id = 1,
                Asignatura = new AsignaturaErpAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString()
                    }
                },
                Plan = new PlanAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    Estudio = new EstudioAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        Tipo = new TipoEstudioAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                        },
                        AreaAcademica = new AreaAcademicaAcademicoModel
                        {
                            Centro = new CentroAcademicoModel
                            {
                                Universidad = new UniversidadAcademicoModel
                                {
                                    Id = 1,
                                    Acronimo = Guid.NewGuid().ToString()
                                }
                            }
                        }
                    }
                }
            };
            mockIErpAcademicoServiceClient.Setup(x => x.GetAsignaturaPlanById(It.IsAny<int>()))
                .ReturnsAsync(asignatura);

            var tipoNivelUso = new TipoNivelUso
            {
                Id = TipoNivelUso.AsignaturaPlan
            };

            var nivelUsoComportamiento = new NivelUsoComportamientoExpedienteDto
            {
                IdRefAsignatura = "1",
            };

            //ACT
            var actual = await sutMock.Object.GetAsignaturaPlanNivelDeUso(
                nivelUsoComportamiento, tipoNivelUso);

            //ASSERT
            Assert.Equal(asignatura.Plan.Estudio.AreaAcademica.Centro.Universidad.Acronimo, actual.AcronimoUniversidad);
            Assert.Equal(asignatura.Plan.Estudio.AreaAcademica.Centro.Universidad.Id.ToString(), actual.IdRefUniversidad);
            Assert.Equal(asignatura.Plan.Estudio.Tipo.Id.ToString(), actual.IdRefTipoEstudio);
            Assert.Equal(asignatura.Plan.Estudio.Tipo.Nombre, actual.NombreTipoEstudio);
            Assert.Equal(asignatura.Plan.Estudio.Id.ToString(), actual.IdRefEstudio);
            Assert.Equal(asignatura.Plan.Estudio.Nombre, actual.NombreEstudio);
            Assert.Equal(asignatura.Plan.Id.ToString(), actual.IdRefPlan);
            Assert.Equal(asignatura.Plan.Nombre, actual.NombrePlan);
            Assert.Equal(asignatura.Asignatura.TipoAsignatura.Id.ToString(), actual.IdRefTipoAsignatura);
            Assert.Equal(asignatura.Asignatura.TipoAsignatura.Nombre, actual.NombreTipoAsignatura);
            Assert.Equal(asignatura.Id.ToString(), actual.IdRefAsignaturaPlan);
            Assert.Equal(asignatura.Asignatura.Id.ToString(), actual.IdRefAsignatura);
            Assert.Equal(asignatura.Asignatura.Nombre, actual.NombreAsignatura);
            Assert.Equal(tipoNivelUso.Id, actual.TipoNivelUso.Id);
            mockIErpAcademicoServiceClient.Verify(x => x.GetAsignaturaPlanById(It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
