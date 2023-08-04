using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetAllRequisitosExpedientes
{
    public class GetAllRequisitosExpedientesQueryHandler : IRequestHandler<GetAllRequisitosExpedientesQuery, RequisitosExpedientesListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetAllRequisitosExpedientesQueryHandler(
            IExpedientesContext context, 
            IMapper mapper,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<RequisitosExpedientesListItemDto[]> Handle(GetAllRequisitosExpedientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.RequisitosExpedientes.Where(r => !r.Bloqueado).AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(ee => ee.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var requisitos = await query
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<RequisitosExpedientesListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            await GetModosRequerimientoDocumentacion(requisitos);
            return requisitos;
        }

        protected internal virtual async Task GetModosRequerimientoDocumentacion(
            RequisitosExpedientesListItemDto[] requisitos)
        {
            if (!requisitos.Any() ||
                requisitos.All(r => !r.IdRefModoRequerimientoDocumentacion.HasValue))
                return;

            var parameters = new ModoRequerimientoDocumentacionListParameters
            {
                NoPaged = true
            };
            var resultModosRequerimientos = await _erpAcademicoServiceClient
                .GetModosRequerimientoDocumentacion(parameters);
            if (!resultModosRequerimientos.Any()) return;

            requisitos
                .Where(r => r.IdRefModoRequerimientoDocumentacion.HasValue)
                .ToList()
                .ForEach(mrd =>
                {
                    mrd.ModoRequerimientoDocumentacion = resultModosRequerimientos
                        .First(r => r.Id == mrd.IdRefModoRequerimientoDocumentacion.Value).Nombre;
                });
        }
    }
}
