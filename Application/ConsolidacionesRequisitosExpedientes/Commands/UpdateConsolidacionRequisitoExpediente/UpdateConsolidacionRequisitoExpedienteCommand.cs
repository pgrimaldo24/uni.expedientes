using MediatR;
using System;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.UpdateConsolidacionRequisitoExpediente
{
    public class UpdateConsolidacionRequisitoExpedienteCommand : IRequest
    {
        public int Id { get; set; }
        public bool EsDocumentacionFisica { get; set; }
        public bool EnviadaPorAlumno { get; set; }
        public DateTime? Fecha { get; set; }
        public string IdRefIdioma { get; set; }
        public string NivelIdioma { get; set; }
        public string Texto { get; set; }
        public int? IdCausaEstadoRequisito { get; set; }
    }
}
