using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseRequerimientoPlanErpAcademico
    {
        public ResponseRequerimientoPlanErpAcademico()
        {
            RequerimientosTiposAsignaturas = new List<ResponseRequerimientoTipoAsignaturaErpAcademico>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosRequerido { get; set; }
        public double? CreditosObtenidos { get; set; }
        public List<ResponseRequerimientoTipoAsignaturaErpAcademico> RequerimientosTiposAsignaturas { get; set; }
    }
}
