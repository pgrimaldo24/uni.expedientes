using System;
using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente
{
    public class AsignaturaExpedienteListItemDto : IMapFrom<AsignaturaExpediente>
    {
        public int Id { get; set; }
        public string IdRefAsignaturaPlan { get; set; }
        public string NombreAsignatura { get; set; }
        public string CodigoAsignatura { get; set; }
        public string IdRefTipoAsignatura { get; set; }
        public double Ects { get; set; }
        public int AnyoAcademicoInicio { get; set; }
        public int AnyoAcademicoFin { get; set; }
        public string IdRefCurso { get; set; }
        public int NumeroCurso { get; set; }
        public string DuracionPeriodo { get; set; }
        public string SimboloIdiomaImparticion { get; set; }
        public string Calificacion { get; set; }
        public ICollection<AsignaturaCalificacionDto> AsignaturasCalificaciones { get; set; }
    }

    public class AsignaturaCalificacionDto : IMapFrom<AsignaturaCalificacion>
    {
        public int Id { get; set; }
        public DateTime? FechaPublicado { get; set; }
        public DateTime? FechaConfirmado { get; set; }
        public int IdRefPeriodoLectivo { get; set; }
        public string Ciclo { get; set; }
        public string AnyoAcademico { get; set; }
        public decimal? Calificacion { get; set; }
        public string NombreCalificacion { get; set; }
        public bool Superada { get; set; }
        public int Convocatoria { get; set; }
    }
}
