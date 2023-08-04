using System.Collections.Generic;
using System.Text.Json.Serialization;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class DatosGestorAlumnoPuedeTitularseErpAcademicoModel
    {
        public DatosGestorAlumnoPuedeTitularseErpAcademicoModel()
        {
            ErroresReconocimientosOrigen = new List<string>();
        }

        public string IdAsignaturaGestor { get; set; }
        public string IdAsignatura { get; set; }
        public string Ects { get; set; }
        public string NotaNumerica { get; set; }
        public string NotaAlfanumerica { get; set; }
        public bool Superado { get; set; }
        public string AnyoAcademico { get; set; }
        public string Convocatoria { get; set; }
        public string Observaciones { get; set; }
        public List<ReconocimientosOrigenModel> ReconocimientosOrigen { get; set; }
        [JsonIgnore]
        public List<string> ErroresReconocimientosOrigen { get; set; }
    }
}
