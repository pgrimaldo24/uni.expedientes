using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById
{
    [Collection("CommonTestCollection")]
    public class GetComportamientoExpedienteByIdQueryHandlerTests : TestBase
    {
        private readonly IMapper _mapper;

        public GetComportamientoExpedienteByIdQueryHandlerTests(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando no existe el comportamiento Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new GetComportamientoExpedienteByIdQuery(1);
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotFoundException>(ex);
        }

        [Fact(DisplayName = "Cuando se encuentra el comportamiento devuelve Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var niveles = new List<NivelUsoComportamientoExpediente>
            {
                new()
                {
                    Id = 1,
                    ComportamientoExpedienteId = 1
                }
            };

            var comportamiento = new ComportamientoExpediente
            {
                Id = 1,
                NivelesUsoComportamientosExpedientes = niveles
            };
            await Context.ComportamientosExpedientes.AddAsync(comportamiento);
            await Context.SaveChangesAsync();
            var mockErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>();
            var sutMock = new Mock<GetComportamientoExpedienteByIdQueryHandler>(Context, _mapper, 
                    mockErpAcademicoServiceClient.Object) { CallBase = true };
            sutMock.Setup(x => x.GetModoRequerimientoDocumentacion(It.IsAny<ComportamientoExpedienteDto>()))
                .Returns(Task.CompletedTask);
            sutMock.Setup(x => x.GetDisplayNameNodoNivelVersion(It.IsAny<NivelUsoComportamientoExpedienteDto>()))
                .Returns(string.Empty);

            //ACT
            var actual = await sutMock.Object.Handle(new GetComportamientoExpedienteByIdQuery(1),
                CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<ComportamientoExpedienteDto>(actual);
            Assert.Equal(Context.ComportamientosExpedientes.FirstOrDefault()?.Id, comportamiento.Id);
            sutMock.Verify(x => 
                x.GetModoRequerimientoDocumentacion(It.IsAny<ComportamientoExpedienteDto>()), Times.Once);
            sutMock.Verify(x =>
                x.GetDisplayNameNodoNivelVersion(It.IsAny<NivelUsoComportamientoExpedienteDto>()), Times.Once);
        }

        #endregion

        #region GetModoRequerimientoDocumentacion

        [Fact(DisplayName = "Cuando los valores de IdRefModoRequerimientoDocumentacion son null Termina el proceso")]
        public async Task GetModoRequerimientoDocumentacion_IdRefModoRequerimientoDocumentacion_Null()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var sutMock = new Mock<GetComportamientoExpedienteByIdQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object) { CallBase = true };
            var comportamiento = new ComportamientoExpedienteDto
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = null
                        }
                    },new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = null
                        }
                    }
                }
            };

            //ACT
            await sutMock.Object.GetModoRequerimientoDocumentacion(comportamiento);

            //ASSERT
            Assert.True(comportamiento.RequisitosComportamientosExpedientes.All(r 
                => !r.RequisitoExpediente.IdRefModoRequerimientoDocumentacion.HasValue));
        }

        [Fact(DisplayName = "Cuando el servicio externo no devuelve información Termina el proceso")]
        public async Task GetModoRequerimientoDocumentacion_Servicio_Empty()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var modosComportamientos = new List<ModoRequerimientoDocumentacionAcademicoModel>();
            mockIErpAcademicoServiceClient.Setup(x =>
                    x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()))
                .ReturnsAsync(modosComportamientos);

            var sutMock = new Mock<GetComportamientoExpedienteByIdQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object)
                { CallBase = true };
            var comportamiento = new ComportamientoExpedienteDto
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = 1
                        }
                    },new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = 2
                        }
                    }
                }
            };

            //ACT
            await sutMock.Object.GetModoRequerimientoDocumentacion(comportamiento);

            //ASSERT
            Assert.Empty(modosComportamientos);
            mockIErpAcademicoServiceClient.Verify(x =>
                x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando se obtienen los nombres de los modos de requerimiento documentación Devuelve Ok")]
        public async Task GetModoRequerimientoDocumentacion_Ok()
        {
            //ARRANGE
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };
            var modosComportamientos = new List<ModoRequerimientoDocumentacionAcademicoModel>
            {
                new()
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString()
                },
                new()
                {
                    Id = 2,
                    Nombre = Guid.NewGuid().ToString()
                }
            };
            mockIErpAcademicoServiceClient.Setup(x =>
                    x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()))
                .ReturnsAsync(modosComportamientos);

            var sutMock = new Mock<GetComportamientoExpedienteByIdQueryHandler>(Context, _mapper,
                    mockIErpAcademicoServiceClient.Object) { CallBase = true };
            var comportamiento = new ComportamientoExpedienteDto
            {
                RequisitosComportamientosExpedientes = new List<RequisitoComportamientoExpedienteDto>
                {
                    new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = 1
                        }
                    },new()
                    {
                        RequisitoExpediente = new RequisitoExpedienteDto
                        {
                            IdRefModoRequerimientoDocumentacion = 2
                        }
                    }
                }
            };

            //ACT
            await sutMock.Object.GetModoRequerimientoDocumentacion(comportamiento);

            //ASSERT
            Assert.True(comportamiento.RequisitosComportamientosExpedientes.All(r 
                => !string.IsNullOrEmpty(r.RequisitoExpediente.ModoRequerimientoDocumentacion)));
            mockIErpAcademicoServiceClient.Verify(x =>
                x.GetModosRequerimientoDocumentacion(It.IsAny<ModoRequerimientoDocumentacionListParameters>()), Times.Once);
        }

        #endregion

        #region GetDisplayNameNodoNivelVersion

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso Universidad Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Universidad_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Universidad
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayNameUniversidad, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso TipoEstudio Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Tipo_Estudio_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoEstudio
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNameTipoEstudio = $"| {nivelUsoComportamientoExpedienteDto.IdRefTipoEstudio} - {nivelUsoComportamientoExpedienteDto.NombreTipoEstudio}";
            var displayName = displayNameUniversidad + displayNameTipoEstudio;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso Estudio Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Estudio_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Estudio
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNameEstudio = $"| {nivelUsoComportamientoExpedienteDto.IdRefEstudio} - {nivelUsoComportamientoExpedienteDto.NombreEstudio}";
            var displayName = displayNameUniversidad + displayNameEstudio;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso PlanEstudio Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Plan_Estudio_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.PlanEstudio
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNamePlan = $"| {nivelUsoComportamientoExpedienteDto.IdRefPlan} - {nivelUsoComportamientoExpedienteDto.NombrePlan}";
            var displayName = displayNameUniversidad + displayNamePlan;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso TipoAsignatura Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Tipo_Asignatura_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = Guid.NewGuid().ToString(),
                NombreTipoAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoAsignatura
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNameTipoAsignatura = $"| {nivelUsoComportamientoExpedienteDto.IdRefTipoAsignatura} - {nivelUsoComportamientoExpedienteDto.NombreTipoAsignatura}";
            var displayName = displayNameUniversidad + displayNameTipoAsignatura;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso AsignaturaPlan Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Asignatura_Plan_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefAsignaturaPlan = Guid.NewGuid().ToString(),
                NombreAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new TipoNivelUsoDto
                {
                    Id = TipoNivelUso.AsignaturaPlan
                }
            };
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNameAsignaturaPlan = $"| {nivelUsoComportamientoExpedienteDto.IdRefAsignaturaPlan}";
            var displayNameAsignatura = $"| {nivelUsoComportamientoExpedienteDto.NombreAsignatura}";
            var displayName = displayNameUniversidad + displayNameAsignaturaPlan + displayNameAsignatura;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        [Fact(DisplayName = "Cuando el TipoNivelUso no existe Devuelve NotImplementedException")]
        public void GetDisplayNameNodoNivelVersion_NotImplementedException()
        {
            //ARRANGE
            var sut = new GetComportamientoExpedienteByIdQueryHandler(Context, _mapper, null);

            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto
            {
                TipoNivelUso = new()
                {
                    Id = 7
                }
            };

            //ACT
            var ex = Record.Exception(() =>
            {
                sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<NotImplementedException>(ex);
        }

        #endregion
    }
}
