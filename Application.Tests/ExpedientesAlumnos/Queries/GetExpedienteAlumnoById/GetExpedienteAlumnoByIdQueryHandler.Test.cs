using System;
using System.Collections.Generic;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById
{
    [Collection("CommonTestCollection")]
    public class GetExpedienteAlumnoByIdQueryHandlerTest : TestBase
    {
        private readonly IMapper _mapper;

        public GetExpedienteAlumnoByIdQueryHandlerTest(CommonTestFixture fixture)
        {
            _mapper = fixture.Mapper;
        }

        #region Handle

        [Fact(DisplayName = "Cuando se obtiene el expediente Devuelve dto")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var expedienteEsperado = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456",
                AlumnoNombre = Guid.NewGuid().ToString(),
                AlumnoApellido1 = Guid.NewGuid().ToString(),
                AlumnoApellido2 = Guid.NewGuid().ToString(),
                IdRefTipoDocumentoIdentificacionPais = Guid.NewGuid().ToString(),
                AlumnoNroDocIdentificacion = Guid.NewGuid().ToString(),
                AlumnoEmail = Guid.NewGuid().ToString(),
                IdRefViaAccesoPlan = Guid.NewGuid().ToString(),
                DocAcreditativoViaAcceso = Guid.NewGuid().ToString(),
                IdRefIntegracionDocViaAcceso = Guid.NewGuid().ToString(),
                FechaSubidaDocViaAcceso = Guid.NewGuid().ToString(),
                IdRefTipoVinculacion = Guid.NewGuid().ToString(),
                NombrePlan = Guid.NewGuid().ToString(),
                IdRefUniversidad = Guid.NewGuid().ToString(),
                AcronimoUniversidad = Guid.NewGuid().ToString(),
                IdRefCentro = Guid.NewGuid().ToString(),
                IdRefAreaAcademica = Guid.NewGuid().ToString(),
                IdRefTipoEstudio = Guid.NewGuid().ToString(),
                IdRefEstudio = Guid.NewGuid().ToString(),
                NombreEstudio = Guid.NewGuid().ToString(),
                IdRefTitulo = Guid.NewGuid().ToString(),
                FechaApertura = DateTime.Now,
                FechaFinalizacion = DateTime.Now,
                FechaTrabajoFinEstudio = DateTime.Now,
                TituloTrabajoFinEstudio = Guid.NewGuid().ToString(),
                FechaExpedicion = DateTime.Now,
                NotaMedia = Double.MaxValue,
                FechaPago = DateTime.Now
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteEsperado);
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetExpedienteAlumnoByIdQuery(1);
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient>();
            var sutMock = new Mock<GetExpedienteAlumnoByIdQueryHandler>(Context, _mapper, 
                mockErpAcademicoService.Object, mockCommonsServiceClient.Object) { CallBase = true };
            mockErpAcademicoService.Setup(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()))
                .ReturnsAsync(new AlumnoAcademicoModel
                {
                    Id = 1,
                    Persona = new PersonaErpAcademicoModel()
                });
            mockErpAcademicoService.Setup(x => x.GetFotoAlumnoById(It.IsAny<int>()))
                .ReturnsAsync(string.Empty);
            sutMock.Setup(x => x.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<AlumnoAcademicoModel>())).ReturnsAsync(new AlumnoDto());
            sutMock.Setup(x => x.GetEspecializacionesDisplayName(
                It.IsAny<List<ExpedienteEspecializacion>>()))
                .ReturnsAsync(new List<ExpedienteEspecializacionDto>());

            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Equal(expedienteEsperado.Id, actual.Id);
            Assert.Equal(expedienteEsperado.IdRefIntegracionAlumno, actual.IdRefIntegracionAlumno);
            Assert.Equal(expedienteEsperado.IdRefPlan, actual.IdRefPlan);
            Assert.Equal(expedienteEsperado.AlumnoNombre, actual.AlumnoNombre);
            Assert.Equal(expedienteEsperado.AlumnoApellido1, actual.AlumnoApellido1);
            Assert.Equal(expedienteEsperado.AlumnoApellido2, actual.AlumnoApellido2);
            Assert.Equal(expedienteEsperado.IdRefTipoDocumentoIdentificacionPais, actual.IdRefTipoDocumentoIdentificacionPais);
            Assert.Equal(expedienteEsperado.AlumnoNroDocIdentificacion, actual.AlumnoNroDocIdentificacion);
            Assert.Equal(expedienteEsperado.AlumnoEmail, actual.AlumnoEmail);
            Assert.Equal(expedienteEsperado.IdRefViaAccesoPlan, actual.IdRefViaAccesoPlan);
            Assert.Equal(expedienteEsperado.DocAcreditativoViaAcceso, actual.DocAcreditativoViaAcceso);
            Assert.Equal(expedienteEsperado.IdRefIntegracionDocViaAcceso, actual.IdRefIntegracionDocViaAcceso);
            Assert.Equal(expedienteEsperado.FechaSubidaDocViaAcceso, actual.FechaSubidaDocViaAcceso);
            Assert.Equal(expedienteEsperado.IdRefTipoVinculacion, actual.IdRefTipoVinculacion);
            Assert.Equal(expedienteEsperado.NombrePlan, actual.NombrePlan);
            Assert.Equal(expedienteEsperado.IdRefUniversidad, actual.IdRefUniversidad);
            Assert.Equal(expedienteEsperado.AcronimoUniversidad, actual.AcronimoUniversidad);
            Assert.Equal(expedienteEsperado.IdRefCentro, actual.IdRefCentro);
            Assert.Equal(expedienteEsperado.IdRefAreaAcademica, actual.IdRefAreaAcademica);
            Assert.Equal(expedienteEsperado.IdRefTipoEstudio, actual.IdRefTipoEstudio);
            Assert.Equal(expedienteEsperado.IdRefEstudio, actual.IdRefEstudio);
            Assert.Equal(expedienteEsperado.NombreEstudio, actual.NombreEstudio);
            Assert.Equal(expedienteEsperado.IdRefTitulo, actual.IdRefTitulo);
            Assert.Equal(expedienteEsperado.FechaApertura, actual.FechaApertura);
            Assert.Equal(expedienteEsperado.FechaFinalizacion, actual.FechaFinalizacion);
            Assert.Equal(expedienteEsperado.FechaTrabajoFinEstudio, actual.FechaTrabajoFinEstudio);
            Assert.Equal(expedienteEsperado.TituloTrabajoFinEstudio, actual.TituloTrabajoFinEstudio);
            Assert.Equal(expedienteEsperado.FechaExpedicion, actual.FechaExpedicion);
            Assert.Equal(expedienteEsperado.NotaMedia, actual.NotaMedia);
            Assert.Equal(expedienteEsperado.FechaPago, actual.FechaPago);
            mockErpAcademicoService.Verify(x => x.GetAlumnoMatriculasDocumentos(It.IsAny<int>()), Times.Once);
            mockErpAcademicoService.Verify(x => x.GetFotoAlumnoById(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.GetAlumnoInfo(It.IsAny<ExpedienteAlumno>(),
                It.IsAny<AlumnoAcademicoModel>()), Times.Once);
            sutMock.Verify(x => x.GetEspecializacionesDisplayName(
                It.IsAny<List<ExpedienteEspecializacion>>()), Times.Once);
        }

        [Fact(DisplayName = "Cuando no se obtiene el expediente Devuelve excepción")]
        public async Task Handle_NotFound()
        {
            //ARRANGE
            var expedienteEsperado = new ExpedienteAlumno
            {
                Id = 1,
                IdRefIntegracionAlumno = "123456",
                IdRefPlan = "789456"
            };
            await Context.ExpedientesAlumno.AddAsync(expedienteEsperado);
            await Context.SaveChangesAsync(CancellationToken.None);

            var request = new GetExpedienteAlumnoByIdQuery(2);
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient>();
            var sut = new GetExpedienteAlumnoByIdQueryHandler(Context, _mapper, 
                mockErpAcademicoService.Object, mockCommonsServiceClient.Object);

            //ACT
            var exception = await Record.ExceptionAsync(async () =>
            {
                await sut.Handle(request, CancellationToken.None);
            });

            //ASSERT
            Assert.IsType<NotFoundException>(exception);
            Assert.Equal(new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno).Message,
                exception.Message);
        }

        #endregion

        #region GetEspecializacionesDisplayName

        [Fact(DisplayName = "Cuando no tiene especializaciones Devuelve lista vacía")]
        public async Task GetEspecializacionesDisplayName_ListaVacia()
        {
            //ARRANGE
            var request = new List<ExpedienteEspecializacion>();
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            mockErpAcademicoService.Setup(s => s.GetEspecializaciones(It.IsAny<EspecializacionListParameters>()))
                .ReturnsAsync(new List<EspecializacionIntegrationModel>());
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient>();
            var sut = new GetExpedienteAlumnoByIdQueryHandler(Context, _mapper, 
                mockErpAcademicoService.Object, mockCommonsServiceClient.Object);

            //ACT
            var actual = await sut.GetEspecializacionesDisplayName(request);

            //ASSERT
            mockErpAcademicoService.Verify(s => s.GetEspecializaciones(It.IsAny<EspecializacionListParameters>()),
                Times.Never);
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Cuando no tiene especializaciones Devuelve lista con los datos de las especializaciones")]
        public async Task GetEspecializacionesDisplayName_ListaConDatos()
        {
            //ARRANGE
            var request = new List<ExpedienteEspecializacion>
            {
                new()
                {
                    Id = 1,
                    IdRefEspecializacion = "10"
                }
            };
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var especializacionIntegrationModelEsperado = new EspecializacionIntegrationModel
            {
                Id = 10,
                DisplayName = Guid.NewGuid().ToString()
            };
            mockErpAcademicoService.Setup(s => s.GetEspecializaciones(It.IsAny<EspecializacionListParameters>()))
                .ReturnsAsync(new List<EspecializacionIntegrationModel>
                {
                    especializacionIntegrationModelEsperado
                });
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient>();
            var sut = new GetExpedienteAlumnoByIdQueryHandler(Context, _mapper, 
                mockErpAcademicoService.Object, mockCommonsServiceClient.Object);

            //ACT
            var actual = await sut.GetEspecializacionesDisplayName(request);

            //ASSERT
            mockErpAcademicoService.Verify(s => s.GetEspecializaciones(It.IsAny<EspecializacionListParameters>()),
                Times.Once);
            Assert.NotEmpty(actual);
            Assert.Equal(especializacionIntegrationModelEsperado.Id, actual[0].Id);
            Assert.Equal(especializacionIntegrationModelEsperado.DisplayName, actual[0].DisplayName);
        }

        #endregion

        #region GetAlumnoInfo

        [Fact(DisplayName = "Cuando se setean los datos del alumno Devuelve el objeto")]
        public async Task GetAlumnoInfo_Ok()
        {
            //ARRANGE
            var mockErpAcademicoService = new Mock<IErpAcademicoServiceClient>();
            var mockCommonsServiceClient = new Mock<ICommonsServiceClient>();
            var sut = new GetExpedienteAlumnoByIdQueryHandler(Context, _mapper,
                mockErpAcademicoService.Object, mockCommonsServiceClient.Object);
            mockErpAcademicoService.Setup(x => x.GetUniversidadById(It.IsAny<int>()))
                .ReturnsAsync(new UniversidadAcademicoModel());
            var expedienteAlumno = new ExpedienteAlumno
            {
                IdRefIntegracionAlumno = Guid.NewGuid().ToString(),
                IdRefUniversidad = "1"
            };
            var alumnoAcademico = new AlumnoAcademicoModel
            {
                Id = 1,
                Persona = new PersonaErpAcademicoModel()
            };

            //ACT
            var actual = await sut.GetAlumnoInfo(expedienteAlumno, alumnoAcademico);

            //ASSERT
            Assert.Equal(alumnoAcademico.Id, actual.IdAlumno);
            Assert.Equal(expedienteAlumno.IdRefIntegracionAlumno, actual.IdIntegracionAlumno);
            mockErpAcademicoService.Verify(x => x.GetUniversidadById(It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
