using MediatR;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.EditRequisito
{
    public class EditRequisitoCommand : IRequest
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public string IdRefUniversidad { get; set; }
        public string Descripcion { get; set; }
        public bool EstaVigente { get; set; }
        public bool RequeridaParaTitulo { get; set; }
        public bool RequiereDocumentacion { get; set; }
        public bool EnviarEmailAlumno { get; set; }
        public bool RequeridaParaPago { get; set; }
        public bool EstaRestringida { get; set; }
        public bool EsCertificado { get; set; }
        public bool RequiereTextoAdicional { get; set; }
        public bool DocumentoSecurizado { get; set; }
        public int? IdRefModoRequerimientoDocumentacion { get; set; }   
        public int IdEstadoExpediente { get; set; }
        public string[] IdsFilesTypes { get; set; }
        public string[] Roles { get; set; }
        public RequisitoExpedienteRequerimientoTituloDto RequisitosExpedientesRequerimientosTitulos { get; set; }
    }

    public class RequisitoExpedienteRequerimientoTituloDto
    {
        public bool RequiereMatricularse { get; set; }
        public int IdTipoRelacionExpediente { get; set; }
    }
}
