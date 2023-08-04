using System;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes
{
    public class SeguimientoExpedienteListItemDto : IMapFrom<SeguimientoExpediente>
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdRefTrabajador { get; set; }
        public string IdCuentaSeguridad { get; set; }
        public string Descripcion { get; set; }
        public string NombreTrabajador { get; set; }
        public virtual TipoSeguimientoExpedienteListItemDto TipoSeguimiento { get; set; }
        public virtual ExpedienteAlumnoListItemDto ExpedienteAlumno { get; set; }
    }

    public class TipoSeguimientoExpedienteListItemDto : IMapFrom<TipoSeguimientoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ExpedienteAlumnoListItemDto : IMapFrom<ExpedienteAlumno>
    {
        public int Id { get; set; }
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string PlanEstudioDisplayName { get; set; }
        public string AlumnoNombre { get; set; }
        public string AlumnoApellido1 { get; set; }
        public string AlumnoApellido2 { get; set; }
        public string NombreEstudio { get; set; }
        public string NombrePlan { get; set; }
        public string AlumnoDisplayName => $"{AlumnoNombre} {AlumnoApellido1} {AlumnoApellido2}".TrimEnd();
    }
}
