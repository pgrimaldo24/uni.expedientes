using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.CreateRequisitoExpedienteDocumento
{
    public class CreateRequisitoExpedienteDocumentoCommand : IRequest
    {
        public string NombreDocumento { get; set; }
        public bool DocumentoObligatorio { get; set; }
        public bool DocumentoEditable { get; set; }
        public bool RequiereAceptacionAlumno { get; set; }
        public bool DocumentoSecurizado { get; set; }
        public int IdRequisitoExpediente { get; set; }
    }
}
