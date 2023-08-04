using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class PlanAcademicoModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Version { get; set; }
        public string DisplayName { get; set; }
        public EstudioAcademicoModel Estudio { get; set; }
        public TituloAcademicoModel Titulo { get; set; }
        public ICollection<VersionPlanAcademicoModel> VersionesPlanes { get; set; }
    }
}
