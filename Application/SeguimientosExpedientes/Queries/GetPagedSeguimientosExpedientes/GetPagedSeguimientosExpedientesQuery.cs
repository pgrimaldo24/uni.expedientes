using MediatR;
using System;
using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes
{
    public class GetPagedSeguimientosExpedientesQuery : QueryParameters, IRequest<ResultListDto<SeguimientoExpedienteListItemDto>>
    {
        public int? FilterIdExpedienteAlumno { get; set; }
        public int? FilterIdRefUniversidad { get; set; }
        public List<string> FiltersIdsRefPlan { get; set; }
        public int? FilterIdRefPlan { get; set; }
        public int? FilterIdTipoSeguimiento { get; set; }
        public DateTime? FilterFechaDesde { get; set; }
        public DateTime? FilterFechaHasta { get; set; }
        public string FilterNombreAlumno { get; set; }
        public string FilterPrimerApellido { get; set; }
        public string FilterSegundoApellido { get; set; }
        public string FilterNroDocIdentificacion { get; set; }
        public string FilterIdRefIntegracionAlumno { get; set; }
        public string FilterIdCuentaSeguridad { get; set; }
    }
}
