using System;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio
{
    public class PeriodoAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public AnyoAcademicoModel AnyoAcademico { get; set; }

    }
}
