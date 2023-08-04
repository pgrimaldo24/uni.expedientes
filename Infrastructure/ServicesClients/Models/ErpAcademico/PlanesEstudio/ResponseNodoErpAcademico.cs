using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public sealed class ResponseNodoErpAcademico
    {
        public ResponseNodoErpAcademico()
        {
            Hitos = new List<ResponseHitoErpAcademico>();
            VersionesPlanes = new List<ResponseVersionPlanErpAcademico>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<ResponseHitoErpAcademico> Hitos { get; set; }
        public ResponseTipoNodoErpAcademico Tipo { get; set; }
        public List<ResponseVersionPlanErpAcademico> VersionesPlanes { get; set; }
    }
}
