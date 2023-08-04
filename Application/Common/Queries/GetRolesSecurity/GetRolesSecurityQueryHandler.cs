using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;

namespace Unir.Expedientes.Application.Common.Queries.GetRolesSecurity
{
    public class GetRolesSecurityQueryHandler : IRequestHandler<GetRolesSecurityQuery, string[]>
    {
        private readonly IIdentityService _identityService;
        private readonly ISecurityService _securityService;
        private readonly IStringLocalizer<GetRolesSecurityQueryHandler> _localizer;
        public GetRolesSecurityQueryHandler(IIdentityService identityService, ISecurityService securityService,
            IStringLocalizer<GetRolesSecurityQueryHandler> localizer)
        {
            _identityService = identityService;
            _securityService = securityService;
            _localizer = localizer;
        }
        public async Task<string[]> Handle(GetRolesSecurityQuery request, CancellationToken cancellationToken)
        {
            var infoSecurity = _identityService.GetUserIdentityInfo();
            if (infoSecurity == null)
                throw new BadRequestException(_localizer["La identidad de usuario no existe"]);

            var account = await _securityService.GetAccountByIdAsync(infoSecurity.Id, null);
            if (account == null)
                throw new BadRequestException(_localizer["La cuenta de usuario no existe"]);

            account.Roles = infoSecurity.Roles;
            return account.Roles;
        }
    }
}
