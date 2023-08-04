using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes
{
    public class GetPagedRequisitosExpedientesQueryHandler : 
        IRequestHandler<GetPagedRequisitosExpedientesQuery, ResultListDto<RequisitosExpedientesListItemDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler> _localizer;

        public GetPagedRequisitosExpedientesQueryHandler(
            IExpedientesContext context, 
            IMapper mapper, 
            IErpAcademicoServiceClient erpAcademicoServiceClient,
            IStringLocalizer<GetPagedRequisitosExpedientesQueryHandler> localizer)
        {
            _context = context;
            _mapper = mapper;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _localizer = localizer;
        }

        public async Task<ResultListDto<RequisitosExpedientesListItemDto>> Handle(GetPagedRequisitosExpedientesQuery request, CancellationToken cancellationToken)
        {
            if (!request.HasPaginationInfo)
                throw new BadRequestException(_localizer["Los campos offset y limit son obligatorios"]);

            var resultListDto = new ResultListDto<RequisitosExpedientesListItemDto>();
            var query = _context.RequisitosExpedientes
                .Include(r => r.EstadoExpediente)
                .Include(r => r.RequisitosExpedientesRequerimientosTitulos)
                .AsQueryable();

            query = ApplyQuery(query, request);
            var requisitos = await query
                .Paginate(request)
                .ProjectTo<RequisitosExpedientesListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            await GetModosRequerimientoDocumentacion(requisitos);
            resultListDto.Elements = requisitos;
            resultListDto.TotalElements = await query.CountAsync(cancellationToken);
            return resultListDto;
        }

        protected internal virtual IQueryable<RequisitoExpediente> ApplyQuery(IQueryable<RequisitoExpediente> query,
            GetPagedRequisitosExpedientesQuery request)
        {
            query = query.Where(r => !r.Bloqueado);
            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                query = query.Where(r => r.Nombre.Contains(request.Nombre));
            }

            if (request.EstaVigente.HasValue)
            {
                query = query.Where(r => r.EstaVigente == request.EstaVigente);
            }

            if (request.RequeridoTitulo.HasValue)
            {
                query = query.Where(r => r.RequeridaParaTitulo == request.RequeridoTitulo);
            }

            if (request.RequiereMatricularse.HasValue)
            {
                query = query.Where(r => r.RequisitosExpedientesRequerimientosTitulos
                                        .Any(red => red.RequiereMatricularse == request.RequiereMatricularse));
            }

            if (request.RequeridoParaPago.HasValue)
            {
                query = query.Where(r => r.RequeridaParaPago == request.RequeridoParaPago);
            }

            if (request.IdsEstadosExpedientes != null && request.IdsEstadosExpedientes.Any())
            {
                query = query.Where(r => request.IdsEstadosExpedientes.Contains(r.EstadoExpediente.Id));
            }

            if (request.RequiereDocumentacion.HasValue)
            {
                query = query.Where(r => r.RequiereDocumentacion == request.RequiereDocumentacion);
            }

            if (request.DocumentacionProtegida.HasValue)
            {
                query = query.Where(r => r.RequisitosExpedientesDocumentos
                                        .Any(red => red.DocumentoSecurizado == request.DocumentacionProtegida));
            }

            if (request.IdsModosRequerimientoDocumentacion != null && request.IdsModosRequerimientoDocumentacion.Any())
            {
                query = query.Where(r => request.IdsModosRequerimientoDocumentacion
                                        .Contains(r.IdRefModoRequerimientoDocumentacion.Value));
            }
            return query;
        }

        protected internal virtual async Task GetModosRequerimientoDocumentacion(
            List<RequisitosExpedientesListItemDto> requisitos)
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