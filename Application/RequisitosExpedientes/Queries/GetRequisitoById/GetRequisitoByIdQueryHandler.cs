using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;


namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitoById
{
    public class GetRequisitoByIdQueryHandler : IRequestHandler<GetRequisitoByIdQuery, RequisitoDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetRequisitoByIdQueryHandler(IExpedientesContext context,
            IMapper mapper, IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<RequisitoDto> Handle(GetRequisitoByIdQuery request, CancellationToken cancellationToken)
        {
            var requisito = await _context.RequisitosExpedientes
                .Include(re => re.EstadoExpediente)
                .Include(re => re.RequisitosExpedientesDocumentos)
                .Include(re => re.RolesRequisitosExpedientes)
                .Include(re => re.RequisitosExpedientesRequerimientosTitulos)
                .ThenInclude(rert => rert.TipoRelacionExpediente)
                .Include(re => re.RequisitosExpedientesFilesType)
                .FirstOrDefaultAsync(re => re.Id == request.Id && !re.Bloqueado, cancellationToken);

            if (requisito == null)
                throw new NotFoundException(nameof(RequisitoExpediente), request.Id);

            var result = _mapper.Map<RequisitoExpediente, RequisitoDto>(requisito);
            result.NombreModoRequerimientoDocumentacion = requisito.IdRefModoRequerimientoDocumentacion != null ? 
                (await _erpAcademicoServiceClient.
                    GetModoRequerimientoDocumentacionById(requisito.IdRefModoRequerimientoDocumentacion.ToString())).Nombre : 
                null;

            return result;
        }
    }
}
