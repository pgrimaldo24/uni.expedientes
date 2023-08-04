using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;

namespace Unir.Expedientes.Application.Common.Queries.GetInfoSecurity
{
    public class GetInfoSecurityQueryHandler : IRequestHandler<GetInfoSecurityQuery, InfoSecurityDto>
    {
        private readonly IIdentityService _identityService;
        private const string DefaultListener = "default-listener-account";
        private const string Rabbit = "Rabbit";
        public GetInfoSecurityQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        public async Task<InfoSecurityDto> Handle(GetInfoSecurityQuery request, CancellationToken cancellationToken)
        {
            var result = new InfoSecurityDto();
            var infoSecurity = _identityService.GetUserIdentityInfo();

            if (infoSecurity?.Id == DefaultListener)
                result.IdOrigenExterno = Rabbit;
            else if (infoSecurity?.Login != null && infoSecurity?.Id != null)
                result.IdRefCuentaSeguridad = infoSecurity.Id;
            else
                result.IdOrigenExterno = infoSecurity?.Name;

            result.DisplayName = GetDisplayNameCurrentAccount(infoSecurity);

            return await Task.FromResult(result);
        }
        protected internal virtual string GetDisplayNameCurrentAccount(IdentityModel identity)
        {
            return !string.IsNullOrWhiteSpace(identity?.Name)
                ? identity.Name
                : !string.IsNullOrWhiteSpace(identity?.FirstName)
                    ? $"{identity.FirstName} {identity.Surname}"
                    : !string.IsNullOrWhiteSpace(identity?.Login)
                        ? identity.Login
                        : identity?.Email;
        }
    }
}
