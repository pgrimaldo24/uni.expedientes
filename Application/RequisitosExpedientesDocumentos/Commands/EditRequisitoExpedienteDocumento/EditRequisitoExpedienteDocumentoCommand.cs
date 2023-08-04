using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Commands.EditRequisitoExpedienteDocumento
{
    public class EditRequisitoExpedienteDocumentoCommand : IRequest
    {
        public int Id { get; set; }
        public string NombreDocumento { get; set; }
        public bool DocumentoObligatorio { get; set; }
        public bool DocumentoEditable { get; set; }
        public bool RequiereAceptacionAlumno { get; set; }
        public bool DocumentoSecurizado { get; set; }
        public int IdRequisitoExpediente { get; set; }
    }
}
