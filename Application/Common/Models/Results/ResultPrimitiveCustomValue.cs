using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.Common.Models.Results
{
    public class ResultPrimitiveCustomValue<T> : Result
    {
        public T Value { get; set; }
    }
}
