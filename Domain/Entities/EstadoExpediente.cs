using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class EstadoExpediente : Entity<int>
    {
        public const int Inicial = 1;
        public const int Abierto = 2;
        public const int Cerrado = 3;
        public const int Anulado = 4;
        public const int Inactivo = 5;
        public const int Bloqueado = 6;

        // Propiedades Primitivas
        /// <summary>
        /// Nombre del estado
        /// </summary>
        public string Nombre { get; set; }
        public string Color { get; set; }

        // Propiedades de Navegación
        public virtual ICollection<ExpedienteAlumno> ExpedientesAlumnos { get; set; }
        public virtual ICollection<SeguimientoExpediente> Seguimientos { get; set; }
        public virtual ICollection<TransicionEstadoExpediente> TransicionesOrigen { get; set; }
        public virtual ICollection<TransicionEstadoExpediente> TransicionesDestino { get; set; }
        public virtual ICollection<RequisitoExpediente> RequisitosExpedientes { get; set; }
        public virtual ICollection<TipoSituacionEstado> TiposSituacionEstado { get; set; }
    }
}
