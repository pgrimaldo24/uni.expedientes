using System;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio
{
    public class PeriodoLectivoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public PeriodoAcademicoModel PeriodoAcademico { get; set; }
        public DuracionPeriodoLectivoErpAcademicoModel DuracionPeriodoLectivo { get; set; }
    }
}
