using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.Common.Models.Results
{
    public class ResultHttpTyped : Result
    {
        public ResultHttpTyped()
        {
            Type = ResultType.Ok;
        }
        public ResultType Type { get; set; }
    }

    public enum ResultType
    {
        Ok = 200,
        Created = 201,
        NoContent = 204,
        BadRequest = 400,
        NotFound = 404,
        Conflict = 409,
        InternalServerError = 500, 
        Unauthorized = 401
    }
}
