using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.Common.Queries.GetRolesSecurity;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes
{
    public class GetTiposSolicitudesQueryHandler : IRequestHandler<GetTiposSolicitudesQuery, List<TiposSolicitudesDto>>
    {
        private readonly IExpedicionTitulosServiceClient _expedicionTitulosServiceClient;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<GetTiposSolicitudesQueryHandler> _localizer;
        public GetTiposSolicitudesQueryHandler(IExpedicionTitulosServiceClient expedicionTitulosServiceClient,
             IMapper mapper, IMediator mediator, IStringLocalizer<GetTiposSolicitudesQueryHandler> localizer)
        {
            _expedicionTitulosServiceClient = expedicionTitulosServiceClient;
            _mapper = mapper;
            _mediator = mediator;
            _localizer = localizer;
        }
        public async Task<List<TiposSolicitudesDto>> Handle(GetTiposSolicitudesQuery request, CancellationToken cancellationToken)
        {
            var tipoSolicitudParametros = new TiposSolicitudesParameters { IdsRefUniversidad = request.IdsRefUniversidad, FilterDisplayName = request.Nombre };

            var rolesUsuario = await _mediator.Send(new GetRolesSecurityQuery(), cancellationToken);

            if (!rolesUsuario.Any(rol => rol == AppConfiguration.KeyAdminRole || rol == AppConfiguration.KeyGestorRole))
            {
                throw new BadRequestException(_localizer[$"El usuario no tiene contiene los roles " +
                    $"{AppConfiguration.KeyAdminRole}, {AppConfiguration.KeyGestorRole}"]);
            }

            if (rolesUsuario.Any(rol => rol == AppConfiguration.KeyGestorRole) &&
                !rolesUsuario.Any(rol => rol == AppConfiguration.KeyAdminRole))
            {
                tipoSolicitudParametros.ConFechaPago = false;
            }

            var tiposSolicitudes = await _expedicionTitulosServiceClient.GetTiposSolicitudes(tipoSolicitudParametros);
            return _mapper.Map<List<TiposSolicitudesTitulosModel>, List<TiposSolicitudesDto>>(tiposSolicitudes);
        }
    }
}
