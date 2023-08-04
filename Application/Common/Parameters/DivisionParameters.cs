using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.Common.Parameters
{
    public class DivisionParameters : QueryParameters
    {
        public string Name { get; set; }
        public string IsoCode { get; set; }
        public string LevelCode { get; set; }
        public string SuperEntityCode { get; set; }
    }
}
