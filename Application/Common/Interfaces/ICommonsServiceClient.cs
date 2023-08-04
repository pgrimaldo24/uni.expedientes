using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Commons;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface ICommonsServiceClient
    {
        Task<UniversitiesBindingTypeCommonsModel[]> GetStudentUniversitiesBindingTypes(string code);

        Task<List<EducationalInstitutionsCommonModel>> GetEducationalInstitutions(
            EducationalInstitutionsFiltersParameters parameters);

        Task<List<CountryCommonsModel>> GetCountries(CountryParameters parameters);
        Task<CountryCommonsModel> GetCountry(string countryCode);
        Task<List<DivisionCommonsModel>> GetDivisionsCountries(string isoCode);
        Task<List<LevelDivisionCommonsModel>> GetDivisionCountryServiciosGestorUnir(DivisionParameters parameters);
        Task<List<LevelDivisionCommonsModel>> GetPathDivision(string code);
        Task<UniversitiesModel> GetUniversities(string code);
    }
}
