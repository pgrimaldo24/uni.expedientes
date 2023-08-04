using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.UpdateConsolidacionRequisitoExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Xunit;

namespace Unir.Expedientes.Application.Tests.ConsolidacionesRequisitosExpedientes.Commands.UpdateConsolidacionRequisitoExpediente
{
    [Collection("CommonTestCollection")]
    public class UpdateConsolidacionRequisitoExpedienteCommandHandlerTests : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando no existe la consolidación requisito expediente Devuelve una excepción")]
        public async Task Handle_NotFoundException()
        {
            //ARRANGE
            var request = new UpdateConsolidacionRequisitoExpedienteCommand();
            var sut = new UpdateConsolidacionRequisitoExpedienteCommandHandler(Context, null, null);

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
            var request = new UpdateConsolidacionRequisitoExpedienteCommand
            {
                Id = 1,
                IdRefIdioma = "1"
            };
            await Context.ConsolidacionesRequisitosExpedientes.AddAsync(new ConsolidacionRequisitoExpediente
            {
                Id = 1,
                RequisitoExpediente = new RequisitoExpediente
                {
                    Id = 1
                },
                ConsolidacionesRequisitosExpedientesDocumentos = new List<ConsolidacionRequisitoExpedienteDocumento>()
            });
            await Context.SaveChangesAsync();
            var mockIErpAcademicoServiceClient = new Mock<IErpAcademicoServiceClient>
            {
                CallBase = true
            };

            var sutMock = new Mock<UpdateConsolidacionRequisitoExpedienteCommandHandler>(Context, null, 
                    mockIErpAcademicoServiceClient.Object) { CallBase = true };
            sutMock.Setup(x => x.ValidatePropiedades(
                    It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<UpdateConsolidacionRequisitoExpedienteCommand>()))
                .Returns(Task.CompletedTask);
            mockIErpAcademicoServiceClient.Setup(x => x.GetIdioma(It.IsAny<int>()))
                .ReturnsAsync(new IdiomaAcademicoModel());
            sutMock.Setup(x => x.AssignConsolidacionRequisitoExpediente(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<UpdateConsolidacionRequisitoExpedienteCommand>(), 
                It.IsAny<IdiomaAcademicoModel>()));

            //ACT
            await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            sutMock.Verify(x => x.ValidatePropiedades(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<UpdateConsolidacionRequisitoExpedienteCommand>()), Times.Once);
            mockIErpAcademicoServiceClient.Verify(x => x.GetIdioma(It.IsAny<int>()), Times.Once);
            sutMock.Verify(x => x.AssignConsolidacionRequisitoExpediente(
                It.IsAny<ConsolidacionRequisitoExpediente>(), It.IsAny<UpdateConsolidacionRequisitoExpedienteCommand>(), 
                It.IsAny<IdiomaAcademicoModel>()), Times.Once);
        }

        #endregion

        #region ValidatePropiedades

        [Fact(DisplayName = "Cuando da error en las validaciones de dominio Devuelve excepción")]
        public async Task ValidatePropiedades_Dominio_BadRequestException()
        {
            //ARRANGE
            var request = new UpdateConsolidacionRequisitoExpedienteCommand();
            var mockIStringLocalizer = new Mock<IStringLocalizer<UpdateConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new UpdateConsolidacionRequisitoExpedienteCommandHandler(Context, mockIStringLocalizer.Object, null);

            const string mensajeEsperado = "El estado debe estar en estado Pendiente o Rechazado";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockConsolidacion = new Mock<ConsolidacionRequisitoExpediente>
            {
                CallBase = true
            };
            mockConsolidacion.SetupAllProperties();
            mockConsolidacion.Setup(x => x.VerificarPropiedadesParaActualizar(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(mensajeEsperado);

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedades(mockConsolidacion.Object, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockConsolidacion.Verify(x => x.VerificarPropiedadesParaActualizar(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact(DisplayName = "Cuando no existe la causa estado requisito Devuelve excepción")]
        public async Task ValidatePropiedades_CausaEstado_BadRequestException()
        {
            //ARRANGE
            var request = new UpdateConsolidacionRequisitoExpedienteCommand
            {
                IdCausaEstadoRequisito = 2
            };
            var mockIStringLocalizer = new Mock<IStringLocalizer<UpdateConsolidacionRequisitoExpedienteCommandHandler>>
            {
                CallBase = true
            };
            var sutMock = new UpdateConsolidacionRequisitoExpedienteCommandHandler(Context, mockIStringLocalizer.Object, null);

            const string mensajeEsperado = "No existe la causa del estado del requisito";
            mockIStringLocalizer.Setup(s => s[It.Is<string>(msj => msj == mensajeEsperado)])
                .Returns(new LocalizedString(mensajeEsperado, mensajeEsperado));

            var mockConsolidacion = new Mock<ConsolidacionRequisitoExpediente>
            {
                CallBase = true
            };
            mockConsolidacion.SetupAllProperties();
            mockConsolidacion.Setup(x => x.VerificarPropiedadesParaActualizar(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
            await Context.CausasEstadosRequisitosConsolidadasExpedientes.AddAsync(
                new CausaEstadoRequisitoConsolidadaExpediente
                {
                    Id = 1
                });
            await Context.SaveChangesAsync();

            //ACT
            var ex = await Record.ExceptionAsync(async () =>
            {
                await sutMock.ValidatePropiedades(mockConsolidacion.Object, request);
            });

            //ASSERT
            Assert.NotNull(ex);
            Assert.IsType<BadRequestException>(ex);
            Assert.Equal(mensajeEsperado, ex.Message);
            mockConsolidacion.Verify(x => x.VerificarPropiedadesParaActualizar(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region AssignConsolidacionRequisitoExpediente

        [Fact(DisplayName = "Cuando se setean los datos Devuelve el objeto")]
        public void AssignConsolidacionRequisitoExpediente_Ok()
        {
            //ARRANGE
            var consolidacionRequisitoExpediente = new ConsolidacionRequisitoExpediente
            {
                EsDocumentacionFisica = true,
                RequisitoExpediente = new RequisitoExpediente
                {
                    EsLogro = true
                }
            };
            var request = new UpdateConsolidacionRequisitoExpedienteCommand
            {
                EsDocumentacionFisica = true,
                EnviadaPorAlumno = true,
                Texto = Guid.NewGuid().ToString(),
                NivelIdioma = Guid.NewGuid().ToString(),
                IdCausaEstadoRequisito = 2,
                Fecha = DateTime.Now
            };
            var idioma = new IdiomaAcademicoModel
            {
                Id = 1,
                Nombre = Guid.NewGuid().ToString(),
                Siglas = Guid.NewGuid().ToString()
            };
            var sut = new UpdateConsolidacionRequisitoExpedienteCommandHandler(Context, null, null);

            //ACT
            sut.AssignConsolidacionRequisitoExpediente(consolidacionRequisitoExpediente, request, idioma);

            //ASSERT
            Assert.Equal(consolidacionRequisitoExpediente.EsDocumentacionFisica, request.EsDocumentacionFisica);
            Assert.Equal(consolidacionRequisitoExpediente.EnviadaPorAlumno, request.EnviadaPorAlumno);
            Assert.Equal(consolidacionRequisitoExpediente.Texto, request.Texto);
            Assert.Equal(consolidacionRequisitoExpediente.NivelIdioma, request.NivelIdioma);
            Assert.Equal(consolidacionRequisitoExpediente.CausaEstadoRequisitoConsolidadaExpedienteId, request.IdCausaEstadoRequisito);
            Assert.Equal(consolidacionRequisitoExpediente.Fecha, request.Fecha);
            Assert.Equal(consolidacionRequisitoExpediente.FechaCambioEstado.Date, DateTime.Now.Date);
            Assert.Equal(consolidacionRequisitoExpediente.EstadoRequisitoExpedienteId, EstadoRequisitoExpediente.Pendiente);
            Assert.Equal(consolidacionRequisitoExpediente.IdRefIdioma, idioma.Id.ToString());
            Assert.Equal(consolidacionRequisitoExpediente.NombreIdioma, idioma.Nombre);
            Assert.Equal(consolidacionRequisitoExpediente.SiglasIdioma, idioma.Siglas);
            Assert.Equal(consolidacionRequisitoExpediente.DocumentacionRecibida, consolidacionRequisitoExpediente.EsDocumentacionFisica);
        }

        #endregion
    }
}
