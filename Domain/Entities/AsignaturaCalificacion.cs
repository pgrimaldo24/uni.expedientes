using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class AsignaturaCalificacion : Entity<int>
    {
        public const int IdDuracionPeriodoLectivoMensual = 1;
        public const int IdDuracionPeriodoLectivoBimestral = 2;
        public const int IdDuracionPeriodoLectivoTrimestral = 3;
        public const int IdDuracionPeriodoLectivoCuatrimestral = 4;
        public const int IdDuracionPeriodoLectivoSemestral = 5;
        public const int IdDuracionPeriodoLectivoAnual = 6;

        /// <summary>
        /// Fecha de publicación de la calificación
        /// </summary>
        public DateTime? FechaPublicado { get; set; }
        /// <summary>
        /// Fecha de confirmación de la calificación
        /// </summary>
        public DateTime? FechaConfirmado { get; set; }
        /// <summary>
        /// Identificador del período lectivo asociado a la obtención de la calificación
        /// </summary>
        public int IdRefPeriodoLectivo { get; set; }
        /// <summary>
        /// Ciclo(formado por el año y el número correspondiente a la duración del período lectivo con respecto a la fecha de inicio) asociado al período lectivo
        /// </summary>
        public string Ciclo { get; set; }
        /// <summary>
        /// Año académico (inicio y fin) asociado al período académico del período lectivo
        /// </summary>
        public string AnyoAcademico { get; set; }
        /// <summary>
        /// Valor numérico o porcentual de la calificación
        /// </summary>
        public decimal? Calificacion { get; set; }
        /// <summary>
        /// Nombre de la calificación
        /// </summary>
        public string NombreCalificacion { get; set; }
        /// <summary>
        /// Número de convocatoria de la calificación; por defecto, 1
        /// </summary>
        public int Convocatoria { get; set; }
        /// <summary>
        /// Identificador de la asignatura matriculada asociada
        /// </summary>
        public int? IdRefAsignaturaMatriculada { get; set; }
        /// <summary>
        /// Identificador de la asignatura ofertada asociada
        /// </summary>
        public int? IdRefAsignaturaOfertada { get; set; }
        /// <summary>
        /// Identificador de la plataforma que ha enviado la calificación
        /// </summary>
        public string Plataforma { get; set; }
        /// <summary>
        /// Identificador del grupo asociado a la asignatura matriculada
        /// </summary>
        public int? IdRefGrupoCurso { get; set; }
        /// <summary>
        /// Identificador del usuario(personNumber) que publica o confirma la calificación
        /// </summary>
        public int? IdPublicadorConfirmador { get; set; }
        /// <summary>
        /// Indica si la calificación es o no una matrícula de honor; por defecto, es false
        /// </summary>
        public bool EsMatriculaHonor { get; set; }
        /// <summary>
        /// Indica si la calificación representa un no presentado; por defecto, es false
        /// </summary>
        public bool EsNoPresentado { get; set; }
        /// <summary>
        /// Indica si la calificación representa o no un valor superado; valor false, por defecto.
        /// </summary>
        public bool Superada { get; set; }
        /// <summary>
        /// Identificador del tipo de convocatoria obtenida con la calificación
        /// </summary>
        public int TipoConvocatoriaId { get; set; }
        public virtual TipoConvocatoria TipoConvocatoria { get; set; }
        /// <summary>
        /// Identificador del estado de la calificación
        /// </summary>
        public int EstadoCalificacionId { get; set; }
        public virtual EstadoCalificacion EstadoCalificacion { get; set; }
        /// <summary>
        /// Identificador de la asignatura con la que está relacionada
        /// </summary>
        public int? AsignaturaExpedienteId { get; set; }
        public virtual AsignaturaExpediente AsignaturaExpediente { get; set; }
    }
}
