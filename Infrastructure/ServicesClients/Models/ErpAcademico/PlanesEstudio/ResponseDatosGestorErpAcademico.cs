using System.Collections.Generic;
using Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseDatosGestorErpAcademico
    {
        public string IdAsignaturaGestor { get; set; }
        public string Ects { get; set; }
        public string NotaNumerica { get; set; }
        public string NotaAlfanumerica { get; set; }
        public bool Superado { get; set; }
        public string AnyoAcademico { get; set; }
        public string Convocatoria { get; set; }
        public string Observaciones { get; set; }
        public List<ResponseReconocimientosOrigenModel> ReconocimientosOrigen { get; set; }
    }
}
