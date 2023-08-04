using System;
using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;


namespace Unir.Expedientes.Application.Anotaciones.Queries.GetPagedAnotaciones
{
    public class GetPagedAnotacionesQuery : QueryParameters, IRequest<ResultListDto<AnotacionListItemDto>>
    {
        public int IdExpedienteAlumno { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string Texto { get; set; }
    }
}
