using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Unir.Framework.Crosscutting.BackgroundJob.Persistence;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/background-job")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class BackgroundJobController : ControllerSuperType
    {
        private readonly IJobExtraInfoRepository _jobExtraInfoRepository;

        public BackgroundJobController(IJobExtraInfoRepository jobExtraInfoRepository)
        {
            _jobExtraInfoRepository = jobExtraInfoRepository;
        }

        [Route("{jobIdExtraInfoId:int}")]
        [HttpGet]
        public async Task<ActionResult<JobExtraInfo>>
            GetJobExtraInfoDetailAsync(int jobIdExtraInfoId)
        {
            var result = await Task.FromResult(_jobExtraInfoRepository.Get(jobIdExtraInfoId));
            return Ok(result);
        }
    }
}
