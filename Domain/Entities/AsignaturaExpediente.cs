using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class AsignaturaExpediente : Entity<int>
    {
        public AsignaturaExpediente()
        {
            AsignaturasCalificaciones = new HashSet<AsignaturaCalificacion>();
        }
        /// <summary>
        /// Identificador de la asignatura plan con la que se relaciona; puede no existir ninguna en el caso de ser, por ejemplo, un reconocimiento o servicio social
        /// </summary>
        public string IdRefAsignaturaPlan { get; set; }
        /// <summary>
        /// Nombre de la asignatura
        /// </summary>
        public string NombreAsignatura { get; set; }
        /// <summary>
        /// Código de la asignatura
        /// </summary>
        public string CodigoAsignatura { get; set; }
        /// <summary>
        /// Orden de la asignatura
        /// </summary>
        public int OrdenAsignatura { get; set; }
        /// <summary>
        /// Número de créditos que representa la asignatura
        /// </summary>
        public double Ects { get; set; }
        /// <summary>
        /// Identificador del tipo de asignatura
        /// </summary>
        public string IdRefTipoAsignatura { get; set; }
        /// <summary>
        /// Código del tipo de asignatura
        /// </summary>
        public string SimboloTipoAsignatura { get; set; }
        /// <summary>
        /// Orden del tipo de asignatura
        /// </summary>
        public int OrdenTipoAsignatura { get; set; }
        /// <summary>
        /// Nombre del tipo de asignatura
        /// </summary>
        public string NombreTipoAsignatura { get; set; }
        /// <summary>
        /// Identificador del curso asociado a la asignatura
        /// </summary>
        public string IdRefCurso { get; set; }
        /// <summary>
        /// Número del curso asociado a la asignatura
        /// </summary>
        public int NumeroCurso { get; set; }
        /// <summary>
        /// Año de inicio del curso académico
        /// </summary>
        public int AnyoAcademicoInicio { get; set; }
        /// <summary>
        /// Año de fin del curso académico
        /// </summary>
        public int AnyoAcademicoFin { get; set; }
        /// <summary>
        /// Nombre del período lectivo asociado a la asignatura
        /// </summary>
        public string PeriodoLectivo { get; set; }
        /// <summary>
        /// Nombre de la duración del período lectivo
        /// </summary>
        public string DuracionPeriodo { get; set; }
        /// <summary>
        /// Símbolo de la duración del período lectivo
        /// </summary>
        public string SimboloDuracionPeriodo { get; set; }
        /// <summary>
        /// Identificador del idioma de impartición
        /// </summary>
        public string IdRefIdiomaImparticion { get; set; }
        /// <summary>
        /// Nombre del idioma de impartición
        /// </summary>
        public string SimboloIdiomaImparticion { get; set; }
        /// <summary>
        /// Indica si la asignatura tiene o no un reconocimiento asociado; valor false, por defecto.
        /// </summary>
        public bool Reconocida { get; set; }
        /// <summary>
        /// Indica la situación/estado de la asignatura en el expediente
        /// </summary>
        public int SituacionAsignaturaId { get; set; }
        public virtual SituacionAsignatura SituacionAsignatura { get; set; }
        /// <summary>
        /// Identificador del expediente
        /// </summary>
        public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
        public virtual ICollection<AsignaturaCalificacion> AsignaturasCalificaciones { get; set; }
        public virtual ICollection<AsignaturaExpedienteRelacionada> AsignaturasExpedientesOrigenes { get; set; }
        public virtual ICollection<AsignaturaExpedienteRelacionada> AsignaturasExpedientesDestinos { get; set; }
        
        public virtual List<string> VerificarPropiedadesRequeridosParaCrear()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(NombreAsignatura))
                errors.Add(MensajeRequerido("Nombre Asignatura"));

            if (string.IsNullOrWhiteSpace(CodigoAsignatura))
                errors.Add(MensajeRequerido("Código Asignatura"));

            if (OrdenAsignatura <= 0)
                errors.Add(MensajeRequerido("Orden Asignatura"));

            if (Ects < 0)
                errors.Add(MensajeRequerido("Ects"));

            if (string.IsNullOrWhiteSpace(IdRefTipoAsignatura))
                errors.Add(MensajeRequerido("IdRef Tipo Asignatura"));

            if (string.IsNullOrWhiteSpace(SimboloTipoAsignatura))
                errors.Add(MensajeRequerido("Simbolo Tipo Asignatura"));

            if (OrdenTipoAsignatura <= 0)
                errors.Add(MensajeRequerido("Orden Tipo Asignatura"));

            if (string.IsNullOrWhiteSpace(NombreTipoAsignatura))
                errors.Add(MensajeRequerido("Nombre Tipo Asignatura"));

            if (string.IsNullOrWhiteSpace(IdRefCurso))
                errors.Add(MensajeRequerido("IdRef Curso"));

            if (NumeroCurso <= 0)
                errors.Add(MensajeRequerido("Número Curso"));

            if (AnyoAcademicoInicio <= 0)
                errors.Add(MensajeRequerido("Año académico Inicio"));

            if (AnyoAcademicoFin <= 0)
                errors.Add(MensajeRequerido("Año académico Fin"));

            if (string.IsNullOrWhiteSpace(PeriodoLectivo))
                errors.Add(MensajeRequerido("Periodo Lectivo"));

            if (string.IsNullOrWhiteSpace(DuracionPeriodo))
                errors.Add(MensajeRequerido("Duración Periodo"));

            if (string.IsNullOrWhiteSpace(SimboloDuracionPeriodo))
                errors.Add(MensajeRequerido("Símbolo Duración Periodo"));

            if (string.IsNullOrWhiteSpace(IdRefIdiomaImparticion))
                errors.Add(MensajeRequerido("IdRef Idioma Impartición"));

            if (string.IsNullOrWhiteSpace(SimboloIdiomaImparticion))
                errors.Add(MensajeRequerido("Símbolo Idioma Impartición"));

            return errors;
        }

        public string MensajeRequerido(string campo)
        {
            return $"El campo {campo} es requerido";
        }

    }
}
