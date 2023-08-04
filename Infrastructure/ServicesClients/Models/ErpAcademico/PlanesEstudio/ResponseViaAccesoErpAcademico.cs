using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseViaAccesoErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EsVigente { get; set; }
        public string DisplayNameSuperViaAcceso { get; set; }
        public virtual ResponseViaAccesoErpAcademico SuperViaAcceso { get; set; }
        public bool HasSuperViaAcceso { get; set; }
        public virtual IEnumerable<ResponseViaAccesoErpAcademico> SubViasAcceso { get; set; }
        public string DisplayNameFromSuperViaAcceso { get; set; }
        public string DisplayNameClasificacionSuperViaAcceso { get; set; }
        public ResponseClasificacionViaAcceso ClasificacionViaAcceso { get; set; }
        public virtual string DisplayName => $"{Nombre}";

        public virtual string DisplayNameWithClasificacion =>
            $"{(SuperViaAcceso != null ? ClasificacionViaAcceso?.DisplayNameWithCode : string.Empty) + DisplayNameFromSuperViaAcceso}";
    }
}
