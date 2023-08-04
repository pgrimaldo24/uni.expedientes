using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Application.Common.Queries.ErpAcademico.GetViasAccesoTreeNode
{
    public class GetViasAccesoTreeNodeQueryHandler : IRequestHandler<GetViasAccesoTreeNodeQuery, List<ViaAccesoTreeNodeDto>>
    {
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetViasAccesoTreeNodeQueryHandler(IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<List<ViaAccesoTreeNodeDto>> Handle(GetViasAccesoTreeNodeQuery request, CancellationToken cancellationToken)
        {
            var parameters = new ViaAccesoListParameters
            {
                FilterIdNodo = request.FilterIdNodo,
                NoPaged = true,
                FilterEsVigente = true,
                ProjectViasAccesosGroupedBySuperViaAcceso = true
            };

            var viasAcceso = await _erpAcademicoServiceClient.GetViasAcceso(parameters);
            return GetViasAccesoTreeNodo(viasAcceso);
        }

        protected internal virtual List<ViaAccesoTreeNodeDto> GetViasAccesoTreeNodo(List<ViaAccesoAcademicoModel> viasAcceso)
        {
            if (viasAcceso == null)
                return new List<ViaAccesoTreeNodeDto>();
            var viasAccesoTreeNodo = viasAcceso.Select(va => new ViaAccesoTreeNodeDto
            {
                Id = va.Id,
                Name = va.DisplayNameWithClasificacion,
                Selectable = va.SubViasAcceso != null && !va.SubViasAcceso.Any(),
                Nodes = GetViasAccesoTreeNodo(va.SubViasAcceso?.ToList())
            }).ToList();
            return viasAccesoTreeNodo;
        }
    }
}
