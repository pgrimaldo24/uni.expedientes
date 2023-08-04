using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion
{
    public class AlumnoMatricula
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public AlumnoAcademicoModel AlumnoAcademicoModel { get; set; }
        public MatriculaAcademicoModel MatriculaAcademicoModel { get; set; }
    }
}
