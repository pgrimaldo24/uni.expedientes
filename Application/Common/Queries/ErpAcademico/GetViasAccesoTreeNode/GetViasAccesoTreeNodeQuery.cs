using System.Collections.Generic;
using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.Common.Queries.ErpAcademico.GetViasAccesoTreeNode
{
    public class GetViasAccesoTreeNodeQuery : QueryParameters, IRequest<List<ViaAccesoTreeNodeDto>>
    {
        public int? FilterIdNodo { get; set; }
    }
}
