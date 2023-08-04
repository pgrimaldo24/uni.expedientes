using MediatR;
using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes
{
    public class GetPagedRequisitosExpedientesQuery : QueryParameters, IRequest<ResultListDto<RequisitosExpedientesListItemDto>>
    {
        public string Nombre { get; set; }
        public bool? EstaVigente { get; set; }
        public bool? RequeridoTitulo { get; set; }
        public bool? RequiereMatricularse { get; set; }
        public bool? RequeridoParaPago { get; set; }
        public ICollection<int> IdsEstadosExpedientes { get; set; }
        public bool? RequiereDocumentacion { get; set; }
        public bool? DocumentacionProtegida { get; set; }
        public ICollection<int> IdsModosRequerimientoDocumentacion { get; set; }
    }
}
