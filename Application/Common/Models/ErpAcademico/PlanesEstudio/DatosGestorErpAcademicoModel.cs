using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class DatosGestorErpAcademicoModel
    {
        public string IdAsignaturaGestor { get; set; }
        public string Ects { get; set; }
        public string NotaNumerica { get; set; }
        public string NotaAlfanumerica { get; set; }
        public bool Superado { get; set; }
        public string AnyoAcademico { get; set; }
        public string Convocatoria { get; set; }
        public string Observaciones { get; set; }
        public List<ReconocimientosOrigenErpAcademicoModel> ReconocimientosOrigen { get; set; }
    }
}
