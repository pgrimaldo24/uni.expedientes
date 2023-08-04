using MediatR;

namespace Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion
{
    public class GetAlumnoByIdIntegracionQuery : IRequest<AlumnoMatricula>
    {
        public string AlumnoIdIntegracion { get; set; }
        public string MatriculaIdIntegracion { get; set; }
        public GetAlumnoByIdIntegracionQuery(string alumnoIdIntegracion, string matriculaIdIntegracion)
        {
            AlumnoIdIntegracion = alumnoIdIntegracion;
            MatriculaIdIntegracion = matriculaIdIntegracion;
        }
    }
}
