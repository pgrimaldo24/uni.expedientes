using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
	/// <summary>
	/// TipoSeguimientoExpediente
	/// </summary>
    public class TipoSeguimientoExpediente : Entity<int>
	{
        public const int ExpedienteCreado = 1;
        public const int ExpedienteModificadoVersionPlan = 2;
        public const int ExpedienteModificadoViaAcceso = 3;
        public const int ExpedienteModificadoTitulacionAcceso = 4;
		public const int ExpedienteModificadoTipoVinculacion = 5;
        public const int ExpedienteActualizadoEnProcesoMasivo = 6;
        public const int ExpedienteActualizado = 7;
		public const int ExpedienteCerrado = 8;
		public const int ActualizadoPorMigracion = 9;

		// Propiedades Primitivas		
		/// <summary>
		/// Descripción de Nombre
		/// </summary>
		[StringLength(100)]
        public string Nombre { get; set; }
		
		// Propiedades de Navegación		
        public virtual ICollection<SeguimientoExpediente> Seguimientos { get; set; }
		
	}
}
