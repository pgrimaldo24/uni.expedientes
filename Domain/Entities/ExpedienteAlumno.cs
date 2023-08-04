using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    /// <summary>
    /// ExpedienteAlumno
    /// </summary>
    public class ExpedienteAlumno : Entity<int>
    {
        public ExpedienteAlumno()
        {
            Anotaciones = new HashSet<Anotacion>();
            RelacionesExpedientesRelacionadas = new HashSet<RelacionExpediente>();
            AsignaturasExpedientes = new List<AsignaturaExpediente>();
            HitosConseguidos = new List<HitoConseguido>();
            ExpedientesEspecializaciones = new HashSet<ExpedienteEspecializacion>();
            TareasDetalle = new HashSet<TareaDetalle>();
        }

        // Propiedades Primitivas		
        /// <summary>
        /// Descripción de IdRefIntegracionAlumno
        /// </summary>
        [StringLength(50)]
        public string IdRefIntegracionAlumno { get; set; }

        /// <summary>
        /// Descripción de IdRefPlan
        /// </summary>
        [StringLength(50)]
        public string IdRefPlan { get; set; }

        /// <summary>
        /// Descripción de IdRefVersionPlan
        /// </summary>
        [StringLength(50)]
        public string IdRefVersionPlan { get; set; }

        /// <summary>
        /// Descripción de IdRefNodo
        /// </summary>
        [StringLength(50)]
        public string IdRefNodo { get; set; }

        [StringLength(200)]
        public string AlumnoNombre { get; set; }

        [StringLength(200)]
        public string AlumnoApellido1 { get; set; }

        [StringLength(200)]
        public string AlumnoApellido2 { get; set; }

        [StringLength(50)]
        public string IdRefTipoDocumentoIdentificacionPais { get; set; }

        [StringLength(50)]
        public string AlumnoNroDocIdentificacion { get; set; }

        public string AlumnoEmail { get; set; }

        [StringLength(50)]
        public string IdRefViaAccesoPlan { get; set; }

        public string DocAcreditativoViaAcceso { get; set; }

        [StringLength(50)]
        public string IdRefIntegracionDocViaAcceso { get; set; }

        [StringLength(50)]
        public string FechaSubidaDocViaAcceso { get; set; }

        [StringLength(50)]
        public string IdRefTipoVinculacion { get; set; }

        [StringLength(250)]
        public string NombrePlan { get; set; }

        [StringLength(50)]
        public string IdRefUniversidad { get; set; }

        [StringLength(50)]
        public string AcronimoUniversidad { get; set; }

        [StringLength(50)]
        public string IdRefCentro { get; set; }

        [StringLength(50)]
        public string IdRefAreaAcademica { get; set; }

        [StringLength(50)]
        public string IdRefTipoEstudio { get; set; }

        [StringLength(50)]
        public string IdRefEstudio { get; set; }

        [StringLength(200)]
        public string NombreEstudio { get; set; }

        [StringLength(50)]
        public string IdRefTitulo { get; set; }

        public DateTime? FechaApertura { get; set; }

        public DateTime? FechaFinalizacion { get; set; }

        public DateTime? FechaTrabajoFinEstudio { get; set; }

        [StringLength(300)]
        public string TituloTrabajoFinEstudio { get; set; }

        public DateTime? FechaExpedicion { get; set; }

        public double? NotaMedia { get; set; }

        public DateTime? FechaPago { get; set; }


        // Propiedades de Navegación
        public virtual ICollection<SeguimientoExpediente> Seguimientos { get; set; }
        public virtual ICollection<ExpedienteEspecializacion> ExpedientesEspecializaciones { get; set; }

        public int? IdTitulacionAcceso { get; set; }
        public virtual TitulacionAcceso TitulacionAcceso { get; set; }
        public ICollection<Anotacion> Anotaciones { get; protected set; }
        public int? EstadoId { get; set; }
        public virtual EstadoExpediente Estado { get; set; }
        public bool Migrado { get; set; }
        public virtual ICollection<RelacionExpediente> RelacionesExpedientesOrigen { get; protected set; }
        public virtual ICollection<RelacionExpediente> RelacionesExpedientesRelacionadas { get; protected set; }
        public virtual ICollection<HitoConseguido> HitosConseguidos { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
        public virtual ICollection<TipoSituacionEstadoExpediente> TiposSituacionEstadoExpedientes { get; set; }
        public virtual List<AsignaturaExpediente> AsignaturasExpedientes { get; set; }
        public virtual ICollection<TareaDetalle> TareasDetalle { get; set; }
        public virtual void AddSeguimiento(int idTipoSeguimiento, string userId, string descripcion,
            DateTime? fecha = null, string origen = null, string mensaje = null)
        {
            Seguimientos ??= new List<SeguimientoExpediente>();
            Seguimientos.Add(new SeguimientoExpediente
            {
                TipoSeguimientoId = idTipoSeguimiento,
                Descripcion = descripcion,
                IdCuentaSeguridad = userId,
                Fecha = fecha ?? DateTime.UtcNow,
                Mensaje = mensaje,
                OrigenExterno = origen
            });
        }
        public virtual void AddSeguimientoNoUser(int idTipoSeguimiento, string descripcion,
            DateTime? fecha = null, string origen = null, string mensaje = null)
        {
            AddSeguimiento(idTipoSeguimiento, null, descripcion, fecha, origen, mensaje);
        }

        public virtual bool HasEspecializacion(string idRefEspecializacion)
        {
            return ExpedientesEspecializaciones != null &&
                   ExpedientesEspecializaciones.Any(ee => ee.IdRefEspecializacion == idRefEspecializacion);
        }

        public virtual void AddEspecializacion(string idRefEspecializacion)
        {
            if (idRefEspecializacion == null) return;
            ExpedientesEspecializaciones ??= new List<ExpedienteEspecializacion>();
            ExpedientesEspecializaciones.Add(new ExpedienteEspecializacion
            {
                IdRefEspecializacion = idRefEspecializacion,
                ExpedienteAlumnoId = this.Id
            });
        }

        public virtual void AddHitosConseguidos(TipoHitoConseguido tipoHitoConseguido, DateTime fechaInicio, ExpedienteEspecializacion expedienteEspecializacion = null)
        {
            HitosConseguidos ??= new List<HitoConseguido>();
            HitosConseguidos.Add(new HitoConseguido
            {
                TipoConseguidoId = tipoHitoConseguido.Id,
                FechaInicio = fechaInicio,
                Nombre = tipoHitoConseguido.Nombre,
                ExpedienteAlumno = this,
                ExpedienteEspecializacion = expedienteEspecializacion
            });
        }

        public virtual void DeleteEspecializacionesNoIncluidos(string[] idsEspecializacionesIncluidos)
        {
            var expedientesEspecializaciones = idsEspecializacionesIncluidos == null ? ExpedientesEspecializaciones.ToList() :
                ExpedientesEspecializaciones.Where(ee => !idsEspecializacionesIncluidos.Contains(ee.IdRefEspecializacion)).ToList();
            foreach (var especializacion in expedientesEspecializaciones)
            {
                ExpedientesEspecializaciones.Remove(especializacion);
            }
        }

        public virtual void AddTipoSituacionEstadoExpediente(TipoSituacionEstado tipoSituacionEstado, DateTime fechaInicio)
        {
            TiposSituacionEstadoExpedientes ??= new List<TipoSituacionEstadoExpediente>();
            TiposSituacionEstadoExpedientes.Add(new TipoSituacionEstadoExpediente
            {
                TipoSituacionEstado = tipoSituacionEstado,
                Descripcion = tipoSituacionEstado.Nombre,
                FechaInicio = fechaInicio,
                ExpedienteAlumno = this
            });
        }

        public virtual void AddConsolidacionRequisitoExpediente(RequisitoComportamientoExpediente requisitoComportamientoExpediente)
        {
            ConsolidacionesRequisitosExpedientes ??= new List<ConsolidacionRequisitoExpediente>();
            ConsolidacionesRequisitosExpedientes.Add(new ConsolidacionRequisitoExpediente
            {
                ExpedienteAlumnoId = Id,
                RequisitoExpedienteId = requisitoComportamientoExpediente.RequisitoExpedienteId,
                TipoRequisitoExpedienteId = requisitoComportamientoExpediente.TipoRequisitoExpedienteId,
                EstadoRequisitoExpedienteId = EstadoRequisitoExpediente.NoProcesado,
                FechaCambioEstado = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Cambiar estado de situación de las asignaturas
        /// </summary>
        /// <param name="idsAsignaturasPlan">Lista de ids de asignaturas plan afectadas</param>
        /// <param name="situacionAsignaturaId">Id Situación de asignatura, Nueva</param>
        public virtual void CambiarSituacionAsignaturas(List<int> idsAsignaturasPlan, int situacionAsignaturaId)
        {
            var asignaturasAfectadas = AsignaturasExpedientes.Where(ae => idsAsignaturasPlan.Contains(Convert.ToInt32(ae.IdRefAsignaturaPlan))).ToList();
            foreach (var asignatura in asignaturasAfectadas)
                asignatura.SituacionAsignaturaId = situacionAsignaturaId;
        }
    }
}
