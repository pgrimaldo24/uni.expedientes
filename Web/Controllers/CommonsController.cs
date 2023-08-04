using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Commons;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/commons")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class CommonsController : ControllerSuperType
    {
        private readonly ICommonsServiceClient _commonsServiceClient;

        public CommonsController(ICommonsServiceClient commonsServiceClient)
        {
            _commonsServiceClient = commonsServiceClient;
        }

        [Route("{code}/student-university-binding-types")]
        [HttpGet]
        public async Task<ActionResult<UniversitiesBindingTypeCommonsModel[]>> GetExpedienteFromServiciosGestorUnir(
            string code)
        {
            var result =
                await _commonsServiceClient.GetStudentUniversitiesBindingTypes(code);
            return Ok(result);
        }
        [Route("educational-institutions")]
        [HttpPost]
        public async Task<ActionResult<List<EducationalInstitutionsCommonModel>>> GetInstitucionDocentesFromServiciosGestorUnir(
            EducationalInstitutionsFiltersParameters parameters)
        {
            var result =
                await _commonsServiceClient.GetEducationalInstitutions(parameters);
            return Ok(result);
        }

        [Route("countries")]
        [HttpPost]
        public async Task<ActionResult<List<EducationalInstitutionsCommonModel>>> GetCountriesServiciosGestorUnir(
            CountryParameters parameters)
        {
            var result =
                await _commonsServiceClient.GetCountries(parameters);
            return Ok(result);
        }

        [Route("countries/{countryCode}")]
        [HttpGet]
        public async Task<ActionResult<EducationalInstitutionsCommonModel>> GetCountry(
            string countryCode)
        {
            var result =
                await _commonsServiceClient.GetCountry(countryCode);
            return Ok(result);
        }

        [Route("countries/{isoCode}/divisions")]
        [HttpGet]
        public async Task<ActionResult<List<DivisionCommonsModel>>> GetDivisionsCountriesServiciosGestorUnir(
            string isoCode)
        {
            var result =
                await _commonsServiceClient.GetDivisionsCountries(isoCode);
            return Ok(result);
        }

        [Route("countries/division")]
        [HttpPost]
        public async Task<ActionResult<List<LevelDivisionCommonsModel>>> GetDivisionCountryServiciosGestorUnir(
            DivisionParameters parameters)
        {
            var result =
                await _commonsServiceClient.GetDivisionCountryServiciosGestorUnir(parameters);
            return Ok(result);
        }

        [Route("countries/division/{code}/path")]
        [HttpGet]
        public async Task<ActionResult<List<LevelDivisionCommonsModel>>> GetPathDivisionServiciosGestorUnir(
            string code)
        {
            var result =
                await _commonsServiceClient.GetPathDivision(code);
            return Ok(result);
        }

        [Route("universities/{code}")]
        [HttpGet]
        public async Task<ActionResult<UniversitiesModel>> GetUniversityFromServiciosGestorUnir(
            string code)
        {
            var result =
                await _commonsServiceClient.GetUniversities(code);
            return Ok(result);
        }
    }
}
