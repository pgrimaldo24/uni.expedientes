using MediatR;
using System;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente
{
    public class GetPagedSeguimientosExpedientesByIdExpedienteQuery : QueryParameters, IRequest<ResultListDto<SeguimientoExpedienteByIdExpedienteListItemDto>>
    {
        public int? FilterIdExpedienteAlumno { get; set; }
        public int? FilterIdTipoSeguimiento { get; set; }
        public DateTime? FilterFechaDesde { get; set; }
        public DateTime? FilterFechaHasta { get; set; }
        public string FilterDescripcion { get; set; }
    }
}
