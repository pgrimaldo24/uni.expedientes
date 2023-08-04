using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo;
using Unir.Expedientes.Application.Tests.Common;
using Unir.Expedientes.Domain.Entities;
using Xunit;

namespace Unir.Expedientes.Application.Tests.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo
{
    [Collection("CommonTestCollection")]
    public class GetCicloPeriodoLectivoQueryHandlerTest : TestBase
    {
        #region Handle
        [Theory(DisplayName = "Cuando se evalúa el ciclo según idDuracionPeriodoCiclo Retorna Ciclo")]
        [InlineData(2, AsignaturaCalificacion.IdDuracionPeriodoLectivoMensual, "2022-2")]
        [InlineData(2, AsignaturaCalificacion.IdDuracionPeriodoLectivoBimestral, "2022-1")]
        [InlineData(12, AsignaturaCalificacion.IdDuracionPeriodoLectivoTrimestral, "2022-4")]
        [InlineData(7, AsignaturaCalificacion.IdDuracionPeriodoLectivoCuatrimestral, "2022-2")]
        [InlineData(6, AsignaturaCalificacion.IdDuracionPeriodoLectivoSemestral, "2022-1")]
        [InlineData(12, AsignaturaCalificacion.IdDuracionPeriodoLectivoAnual, "2022-1")]
        public async Task Handle_Periodos_Ok(int mes, int idDuracionPeriodoLectivo, string resultado)
        {
            //ARRANGE
            var request = new GetCicloPeriodoLectivoQuery(new DateTime(2022, mes, 1), idDuracionPeriodoLectivo);
            var sut = new GetCicloPeriodoLectivoQueryHandler();

            //ACT
            var result = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.NotNull(result);
            Assert.Equal(resultado, result);
        }

        [Fact(DisplayName = "Cuando el id duración periodo no existe Retorna Nulo")]
        public async Task Handle_Periodo_Nulo()
        {
            //ARRANGE
            var request = new GetCicloPeriodoLectivoQuery(new DateTime(2022, 12, 1), 0);
            var sut = new GetCicloPeriodoLectivoQueryHandler();

            //ACT
            var result = await sut.Handle(request, CancellationToken.None);

            //ASSERT
            Assert.Null(result);
        }
        #endregion
    }
}
