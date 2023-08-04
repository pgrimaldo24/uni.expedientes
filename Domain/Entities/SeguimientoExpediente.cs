using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    /// <summary>
    /// SeguimientoExpediente
    /// </summary>
    public class SeguimientoExpediente : Entity<int>
	{
		// Propiedades Primitivas		
		/// <summary>
		/// Descripción de Fecha
		/// </summary>
        public DateTime Fecha { get; set; }
		
		/// <summary>
		/// Descripción de IdRefTrabajador
		/// </summary>
		public string IdRefTrabajador { get; set; }
		
		/// <summary>
		/// Descripción de Descripcion
		/// </summary>
		public string Descripcion { get; set; }
		public string IdCuentaSeguridad { get; set; }

		/// <summary>
		/// Identificador de una aplicación externa como, por ejemplo: mensaje Rabbit, Gestor, …
		/// </summary>
        public string OrigenExterno { get; set; }
        public string Mensaje { get; set; }

		// Propiedades de Navegación		
		public int TipoSeguimientoId { get; set; }
        public virtual TipoSeguimientoExpediente TipoSeguimiento { get; set; }
		public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
		public int? EstadoId { get; set; }
		public virtual EstadoExpediente Estado { get; set; }
	}
}
