using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.PersistenceSuperTypes;

namespace Unir.Expedientes.Persistence.Context
{
    public class ExpedientesContext : DbContextSuperType, IExpedientesContext
    {
        public ExpedientesContext(DbContextOptions options) : base(options)
        { }

        public DbSet<ExpedienteAlumno> ExpedientesAlumno { get; set; }
        public DbSet<SeguimientoExpediente> SeguimientosExpediente { get; set; }
        public DbSet<TipoSeguimientoExpediente> TipoSeguimientosExpediente { get; set; }
        public DbSet<TitulacionAcceso> TitulacionesAccesos { get; set; }
        public DbSet<ExpedienteEspecializacion> ExpedientesEspecializaciones { get; set; }
        public DbSet<ParametroConfiguracionExpediente> ParametrosConfiguracionesExpedientes { get; set; }
        public DbSet<ParametroConfiguracionExpedienteFileType> ParametrosConfiguracionesExpedientesFilesTypes { get; set; }
        public DbSet<Anotacion> Anotaciones { get; set; }
        public DbSet<RolAnotacion> RolesAnotaciones { get; set; }
        public DbSet<EstadoExpediente> EstadosExpedientes { get; set; }
        public DbSet<TransicionEstadoExpediente> TransicionesEstadosExpedientes { get; set; }
        public DbSet<TipoSituacionEstado> TiposSituacionEstado { get; set; }
        public DbSet<TipoSituacionEstadoExpediente> TiposSituacionEstadoExpedientes { get; set; }
        public DbSet<TipoRelacionExpediente> TiposRelacionesExpediente { get; set; }
        public DbSet<RelacionExpediente> RelacionesExpedientes { get; set; }
        public DbSet<HitoConseguido> HitosConseguidos { get; set; }
        public DbSet<TipoHitoConseguido> TiposHitoConseguidos { get; set; }
        public DbSet<RequisitoExpediente> RequisitosExpedientes { get; set; }
        public DbSet<EstadoRequisitoExpediente> EstadosRequisitosExpedientes { get; set; }
        public DbSet<CausaEstadoRequisitoConsolidadaExpediente> CausasEstadosRequisitosConsolidadasExpedientes { get; set; }
        public DbSet<RolRequisitoExpediente> RolesRequisitosExpedientes { get; set; }
        public DbSet<RequisitoExpedienteDocumento> RequisitosExpedientesDocumentos { get; set; }
        public DbSet<RequisitoExpedienteFileType> RequisitosExpedientesFileType { get; set; }
        public DbSet<ComportamientoExpediente> ComportamientosExpedientes { get; set; }
        public DbSet<RequisitoComportamientoExpediente> RequisitosComportamientosExpedientes { get; set; }
        public DbSet<TipoRequisitoExpediente> TiposRequisitosExpedientes { get; set; }
        public DbSet<NivelUsoComportamientoExpediente> NivelesUsoComportamientosExpedientes { get; set; }
        public DbSet<TipoNivelUso> TiposNivelesUso { get; set; }
        public DbSet<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
        public DbSet<ConsolidacionRequisitoExpedienteDocumento> ConsolidacionesRequisitosExpedientesDocumentos { get; set; }
        public DbSet<SituacionAsignatura> SituacionesAsignaturas { get; set; }
        public DbSet<AsignaturaExpediente> AsignaturasExpedientes { get; set; }
        public DbSet<RequisitoExpedienteRequerimientoTitulo> RequisitosExpedientesRequerimientosTitulos { get; set; }
        public DbSet<ConfiguracionExpedienteUniversidad> ConfiguracionesExpedientesUniversidades { get; set; }
        public DbSet<AsignaturaCalificacion> AsignaturasCalificaciones { get; set; }
        public DbSet<TipoConvocatoria> TiposConvocatorias { get; set; }
        public DbSet<EstadoCalificacion> EstadoCalificaciones { get; set; }
        public DbSet<AsignaturaExpedienteRelacionada> AsignaturasExpedientesRelacionadas { get; set; }
        public virtual DbSet<Tarea> Tareas { get; set; }
        public virtual DbSet<TareaDetalle> TareasDetalle { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
