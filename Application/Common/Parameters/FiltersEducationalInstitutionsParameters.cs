using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class EducationalInstitutionsFiltersParameters : QueryParameters
    {
        public string Nombre { get; set; }
        public string CodeCountry { get; set; }
        public int IndexPage { get; set; }
    }
}
