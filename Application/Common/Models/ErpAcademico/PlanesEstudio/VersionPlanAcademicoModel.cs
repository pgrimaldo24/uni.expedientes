using System;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class VersionPlanAcademicoModel
    {
        public int Id { get; set; }
        public int Nro { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
