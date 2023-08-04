using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion
{
    public class ConsolidacionRequisitoExpedienteDocumentoListItemDto : IMapFrom<ConsolidacionRequisitoExpedienteDocumento>
    {
        public int Id { get; set; }
        public string Fichero { get; set; }
        public DateTime FechaFichero { get; set; }
        public string IdRefDocumento { get; set; }
        public bool FicheroValidado { get; set; }
    }
}
