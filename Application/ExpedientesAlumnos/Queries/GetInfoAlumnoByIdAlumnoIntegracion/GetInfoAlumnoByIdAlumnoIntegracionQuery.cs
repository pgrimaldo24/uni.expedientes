using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion
{
    public class GetInfoAlumnoByIdAlumnoIntegracionQuery : IRequest<AlumnoInfoDto>
    {
        public string IdAlumnoIntegracion { get; set; }

        public GetInfoAlumnoByIdAlumnoIntegracionQuery(string idAlumnoIntegracion)
        {
            IdAlumnoIntegracion = idAlumnoIntegracion;
        }
    }
}
