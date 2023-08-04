using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos
{
    public class ExpedienteAlumnoListItemDto : IMapFrom<ExpedienteAlumno>
    {
        public int Id { get; set; }
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefVersionPlan { get; set; }
        public string IdRefNodo { get; set; }
        public string IdRefTipoVinculacion { get; set; }
        public DateTime? FechaApertura { get; set; }
        public List<AsignaturaExpedienteDto> AsignaturasExpedientes { get; set; }
    }
}
