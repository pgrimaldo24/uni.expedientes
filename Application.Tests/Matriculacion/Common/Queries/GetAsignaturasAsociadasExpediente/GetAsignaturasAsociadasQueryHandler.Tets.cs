using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente
{
    [Collection("CommonTestCollection")]
    public class GetAsignaturasAsociadasQueryHandlerTest : TestBase
    {
        #region Handle

        [Fact(DisplayName = "Cuando es correcto Retorna Ok")]
        public async Task Handle_Ok()
        {
            //ARRANGE
            var listaAsignaturasOfertadas = new List<AsignaturaOfertadaModel>()
            {
                 new AsignaturaOfertadaModel
                    {
                        Id = 1,
                        AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                        {
                            Id = 1
                        },
                    }
            };

            var request = new GetAsignaturasAsociadasQuery(new List<AsignaturaExpediente>(), listaAsignaturasOfertadas, SituacionAsignatura.Matriculada);
            var sutMock = new Mock<GetAsignaturasAsociadasQueryHandler>() { CallBase = true };
            sutMock.Setup(x => x.AssignAsignaturaExpediente(It.IsAny<string>(), It.IsAny<AsignaturaOfertadaModel>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new AsignaturaExpediente());
            //ACT
            var actual = await sutMock.Object.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<List<AsignaturaExpediente>>(actual);
            Assert.True(actual.Any());
            sutMock.Verify(x => x.AssignAsignaturaExpediente(It.IsAny<string>(), It.IsAny<AsignaturaOfertadaModel>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region AssignAsignaturaExpediente

        [Fact(DisplayName = "Cuando es correcto Retorna Entidad")]
        public async Task AssignAsignaturaExpediente()
        {
            //ARRANGE
            var request = new AsignaturaOfertadaModel
            {
                Id = 1,
                AsignaturaPlan = new AsignaturaPlanErpAcademicoModel
                {
                    Id = 1,
                    DisplayName = Guid.NewGuid().ToString(),
                    Asignatura = new AsignaturaErpAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        Codigo = Guid.NewGuid().ToString(),
                        IdiomaImparticion = new IdiomaAcademicoModel
                        {
                            Id = 1,
                            Nombre = Guid.NewGuid().ToString(),
                            Siglas = Guid.NewGuid().ToString(),
                        }
                    },
                    Orden = 1
                },
                TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    Orden = 1
                },
                PeriodoLectivo = new PeriodoLectivoModel
                {
                    Id = 1,
                    Nombre = Guid.NewGuid().ToString(),
                    PeriodoAcademico = new PeriodoAcademicoModel
                    {
                        Id = 1,
                        Nombre = Guid.NewGuid().ToString(),
                        AnyoAcademico = new AnyoAcademicoModel
                        {
                            AnyoInicio = 2020,
                            AnyoFin = 2021
                        }
                    },
                    DuracionPeriodoLectivo = new DuracionPeriodoLectivoErpAcademicoModel
                    {
                        Nombre = Guid.NewGuid().ToString(),
                        Simbolo = Guid.NewGuid().ToString()
                    }
                }
            };

            var mock = new GetAsignaturasAsociadasQueryHandler();
            //ACT
            var actual = mock.AssignAsignaturaExpediente("1", request, SituacionAsignatura.Matriculada, false);

            //ASSERT
            Assert.NotNull(actual);
            Assert.IsType<AsignaturaExpediente>(actual);
        }

        #endregion

    }
}
