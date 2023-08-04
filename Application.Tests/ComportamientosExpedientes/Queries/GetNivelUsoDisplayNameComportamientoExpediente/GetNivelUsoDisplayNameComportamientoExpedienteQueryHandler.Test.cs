using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.AddNivelUsoComportamientoExpedienteUncommit;
using Unir.Expedientes.Application.ComportamientosExpedientes.Commands.CreateComportamientoExpediente;
using Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetNivelUsoDisplayNameComportamientoExpediente;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ComportamientosExpedientes.Queries.GetNivelUsoDisplayNameComportamientoExpediente
{
    [Collection("CommonTestCollection")]
    public class GetNivelUsoDisplayNameComportamientoExpedienteQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetNivelUsoDisplayNameComportamientoExpedienteQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se obtiene el displayName del nivel uso Devuelve ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var nivelUsoComportamientoExpediente = new NivelUsoComportamientoExpediente();
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            mockIMediator.Setup(s => s.Send(It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(nivelUsoComportamientoExpediente);
            var nivelUsoComportamientoExpedienteDto = new NivelUsoComportamientoExpedienteDto();
            var request = new GetNivelUsoDisplayNameComportamientoExpedienteQuery(nivelUsoComportamientoExpedienteDto);
            var sutMock = new Mock<GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler>(mockIMediator.Object, _mapper)
            {
                CallBase = true
            };
            const string displayName = "universidad-estudio";
            sutMock.Setup(s =>
                s.GetDisplayNameNodoNivelVersion(It.IsAny<Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto>())).Returns(displayName);

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            mockIMediator.Verify(s => s.Send(It.IsAny<AddNivelUsoComportamientoExpedienteUncommitCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            sutMock.Verify(s => s.GetDisplayNameNodoNivelVersion(It.IsAny<Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto>()), Times.Once);
            Assert.Equal(displayName, actual);
        }

        #endregion

        #region GetDisplayNameNodoNivelVersion

        [Fact(DisplayName = "Cuando se obtiene el displayName al asignar los datos del nivel de uso Universidad Devuelve ok")]
        public void GetDisplayNameNodoNivelVersion_Universidad_Ok()
        {
            //ARRANGE
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Universidad
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
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
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                NombreTipoEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoEstudio
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
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
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.Estudio
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
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
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefPlan = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.PlanEstudio
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
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
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefTipoAsignatura = Guid.NewGuid().ToString(),
                NombreTipoAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.TipoAsignatura
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
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
            var mockIMediator = new Mock<IMediator>
            {
                CallBase = true
            };
            var nivelUsoComportamientoExpedienteDto = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.NivelUsoComportamientoExpedienteDto
            {
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefAsignaturaPlan = Guid.NewGuid().ToString(),
                NombreAsignatura = Guid.NewGuid().ToString(),
                TipoNivelUso = new Application.ComportamientosExpedientes.Queries.GetComportamientoExpedienteById.TipoNivelUsoDto
                {
                    Id = TipoNivelUso.AsignaturaPlan
                }
            };
            var sut = new GetNivelUsoDisplayNameComportamientoExpedienteQueryHandler(mockIMediator.Object, _mapper);
            var displayNameUniversidad = $"{nivelUsoComportamientoExpedienteDto.IdRefUniversidad} - {nivelUsoComportamientoExpedienteDto.AcronimoUniversidad}";
            var displayNameAsignaturaPlan = $"| {nivelUsoComportamientoExpedienteDto.IdRefAsignaturaPlan}";
            var displayNameAsignatura = $"| {nivelUsoComportamientoExpedienteDto.NombreAsignatura}";
            var displayName = displayNameUniversidad + displayNameAsignaturaPlan + displayNameAsignatura;

            //ACT
            var actual = sut.GetDisplayNameNodoNivelVersion(nivelUsoComportamientoExpedienteDto);

            //ASSERT
            Assert.Equal(displayName, actual);
        }

        #endregion
    }
}
