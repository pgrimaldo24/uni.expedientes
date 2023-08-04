using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IExpedientesContext
    {
        DbSet<ExpedienteAlumno> ExpedientesAlumno { get; set; }
        DbSet<SeguimientoExpediente> SeguimientosExpediente { get; set; }
        DbSet<TipoSeguimientoExpediente> TipoSeguimientosExpediente { get; set; }
        DbSet<TitulacionAcceso> TitulacionesAccesos { get; set; }
        DbSet<ExpedienteEspecializacion> ExpedientesEspecializaciones { get; set; }
        DbSet<ParametroConfiguracionExpediente> ParametrosConfiguracionesExpedientes { get; set; }
        DbSet<ParametroConfiguracionExpedienteFileType> ParametrosConfiguracionesExpedientesFilesTypes { get; set; }
        DbSet<Anotacion> Anotaciones { get; set; }
        DbSet<RolAnotacion> RolesAnotaciones { get; set; }
        DbSet<EstadoExpediente> EstadosExpedientes { get; set; }
        DbSet<TransicionEstadoExpediente> TransicionesEstadosExpedientes { get; set; }
        DbSet<TipoSituacionEstado> TiposSituacionEstado { get; set; }
        DbSet<TipoSituacionEstadoExpediente> TiposSituacionEstadoExpedientes { get; set; }
        DbSet<TipoRelacionExpediente> TiposRelacionesExpediente { get; set; }
        DbSet<RelacionExpediente> RelacionesExpedientes { get; set; }
        DbSet<HitoConseguido> HitosConseguidos { get; set; }
        DbSet<TipoHitoConseguido> TiposHitoConseguidos { get; set; }
        DbSet<RequisitoExpediente> RequisitosExpedientes { get; set; }
        DbSet<EstadoRequisitoExpediente> EstadosRequisitosExpedientes { get; set; }
        DbSet<CausaEstadoRequisitoConsolidadaExpediente> CausasEstadosRequisitosConsolidadasExpedientes { get; set; }
        DbSet<RolRequisitoExpediente> RolesRequisitosExpedientes { get; set; }
        DbSet<RequisitoExpedienteDocumento> RequisitosExpedientesDocumentos { get; set; }
        DbSet<RequisitoExpedienteFileType> RequisitosExpedientesFileType { get; set; }
        DbSet<ComportamientoExpediente> ComportamientosExpedientes { get; set; }
        DbSet<RequisitoComportamientoExpediente> RequisitosComportamientosExpedientes { get; set; }
        DbSet<TipoRequisitoExpediente> TiposRequisitosExpedientes { get; set; }
        DbSet<NivelUsoComportamientoExpediente> NivelesUsoComportamientosExpedientes { get; set; }
        DbSet<TipoNivelUso> TiposNivelesUso { get; set; }
        DbSet<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
        DbSet<ConsolidacionRequisitoExpedienteDocumento> ConsolidacionesRequisitosExpedientesDocumentos { get; set; }
        DbSet<SituacionAsignatura> SituacionesAsignaturas { get; set; }
        DbSet<AsignaturaExpediente> AsignaturasExpedientes { get; set; }
        DbSet<RequisitoExpedienteRequerimientoTitulo> RequisitosExpedientesRequerimientosTitulos { get; set; }
        DbSet<ConfiguracionExpedienteUniversidad> ConfiguracionesExpedientesUniversidades { get; set; }
        DbSet<AsignaturaCalificacion> AsignaturasCalificaciones { get; set; }
        DbSet<TipoConvocatoria> TiposConvocatorias { get; set; }
        DbSet<EstadoCalificacion> EstadoCalificaciones { get; set; }
        DbSet<AsignaturaExpedienteRelacionada> AsignaturasExpedientesRelacionadas { get; set; }
        DbSet<Tarea> Tareas { get; set; }
        DbSet<TareaDetalle> TareasDetalle { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        EntityEntry<TEntity> Remove<TEntity>([NotNull] TEntity entity)
            where TEntity : class;
        EntityEntry Remove([NotNull] object entity);
    }
}
