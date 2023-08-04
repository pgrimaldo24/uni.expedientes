using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class ViaAccesoAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EsVigente { get; set; }
        public string DisplayNameSuperViaAcceso { get; set; }
        public virtual ViaAccesoAcademicoModel SuperViaAcceso { get; set; }
        public bool HasSuperViaAcceso { get; set; }
        public virtual IEnumerable<ViaAccesoAcademicoModel> SubViasAcceso { get; set; }
        public string DisplayNameFromSuperViaAcceso { get; set; }
        public string DisplayNameClasificacionSuperViaAcceso { get; set; }
        public ClasificacionViaAccesoModel ClasificacionViaAcceso { get; set; }
        public virtual string DisplayName => $"{Nombre}";
        public virtual string DisplayNameWithClasificacion =>
            $"{(SuperViaAcceso != null ? ClasificacionViaAcceso?.DisplayNameWithCode : string.Empty) + DisplayNameFromSuperViaAcceso}";
    }
}
