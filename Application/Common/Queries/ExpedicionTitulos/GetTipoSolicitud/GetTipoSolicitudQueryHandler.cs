using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud
{
    public class GetTipoSolicitudQueryHandler : IRequestHandler<GetTipoSolicitudQuery, TipoSolicitudDto>
    {
        private readonly IMapper _mapper;
        private readonly IExpedicionTitulosServiceClient _expedicionTitulosServiceClient;
        private readonly IStringLocalizer<GetTipoSolicitudQueryHandler> _localizer;
        public GetTipoSolicitudQueryHandler(IExpedicionTitulosServiceClient expedicionTitulosServiceClient, IMapper mapper,
            IStringLocalizer<GetTipoSolicitudQueryHandler> localizer)
        {
            _mapper = mapper;
            _expedicionTitulosServiceClient = expedicionTitulosServiceClient;
            _localizer = localizer;
        }

        public async Task<TipoSolicitudDto> Handle(GetTipoSolicitudQuery request, CancellationToken cancellationToken)
        {
            var tiposSolicitudes = await _expedicionTitulosServiceClient.GetTipoSolicitudTituloById(request.Id);
            if (tiposSolicitudes is null)
                throw new BadRequestException(_localizer["El tipo de solicitud de título no ha sido encontrado"]);

            return _mapper.Map<TipoSolicitudTituloModel, TipoSolicitudDto>(tiposSolicitudes);
        }
    }
}
