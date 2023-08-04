using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoSituacionEstado : Entity<int>
    {
        public const int PreMatriculado = 1;
        public const int CanceladaDesestimacionMatricula = 2;
        public const int CanceladaDesestimacionAmpliacionMatricula = 3;
        public const int CanceladaDesestimacionVariacionMatricula = 4;
        public const int CancelacionBajaMatriculaNuevoIngreso = 7;
        public const int AmpliacionMatricula = 8;
        public const int CancelacionBajaAmpliacionMatricula = 9;
        public const int VariacionMatricula = 10;
        public const int VariacionMatriculaAnulada = 11;
        public const int MatriculaDesestimada = 14;
        public const int BajaMatriculaNuevoIngreso = 15;
        public const int AmpliacionMatriculaDesestimada = 16;
        public const int VariacionMatriculaRecuperada = 12;
        public const int VariacionMatriculaDesestimada = 19;
        public const int VariacionMatriculaRecuperadaDejaSinAsignaturas = 21;
        public const int BajaAmpliacionMatricula = 17;
        public const int VariacionMatriculaDejaSinAsignaturas = 18;
        public const int TituloExpedido = 22;
        /// <summary>
        /// Nombre de la situación
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Estado asociado
        /// </summary>
        public EstadoExpediente Estado { get; set; }

        // Propiedades de Navegación
        public virtual ICollection<TipoSituacionEstadoExpediente> TiposSituacionesEstadosExpedientes { get; set; }
    }
}
