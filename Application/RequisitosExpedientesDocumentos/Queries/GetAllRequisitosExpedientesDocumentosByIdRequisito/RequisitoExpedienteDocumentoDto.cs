using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito
{
    public class RequisitoExpedienteDocumentoDto : IMapFrom<RequisitoExpedienteDocumento>
    {
        public int Id { get; set; }
        public string NombreDocumento { get; set; }
        public bool DocumentoObligatorio { get; set; }
        public bool DocumentoEditable { get; set; }
        public bool DocumentoSecurizado { get; set; }
        public bool RequiereAceptacionAlumno { get; set; }
        public string IdRefPlantilla { get; set; }
    }
}
