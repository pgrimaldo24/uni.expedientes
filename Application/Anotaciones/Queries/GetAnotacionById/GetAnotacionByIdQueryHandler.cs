using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;


namespace Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById
{
    public class GetAnotacionByIdQueryHandler : IRequestHandler<GetAnotacionByIdQuery, AnotacionDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly ISecurityService _securityService;

        public GetAnotacionByIdQueryHandler(IExpedientesContext context, 
            IMapper mapper, ISecurityService securityService)
        {
            _context = context;
            _mapper = mapper;
            _securityService = securityService;
        }
        public async Task<AnotacionDto> Handle(GetAnotacionByIdQuery request, CancellationToken cancellationToken)
        {
            var anotacion = await _context.Anotaciones
                .Include(a => a.ExpedienteAlumno)
                .Include(a => a.RolesAnotaciones)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (anotacion == null)
                throw new NotFoundException(nameof(Anotacion), request.Id);

            var result = _mapper.Map<Anotacion, AnotacionDto>(anotacion);
            await GetInformacionCuentaSeguridad(result);
            return result;
        }

        protected internal virtual async Task GetInformacionCuentaSeguridad(AnotacionDto anotacion)
        {
            if (string.IsNullOrEmpty(anotacion.IdRefCuentaSeguridad)) return;

            var account = await _securityService.GetAccountByIdAsync(anotacion.IdRefCuentaSeguridad, null);
            if (account == null) return;
            anotacion.NombreUsuario = $"{account.FirstName} {account.Surname}";
        }
    }
}
