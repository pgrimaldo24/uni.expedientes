using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente
{
    public class HasPrimeraMatriculaExpedienteQuery : IRequest<bool>
    {
        public string IdIntegracionMatricula { get; set; }
        public HasPrimeraMatriculaExpedienteQuery(string idIntegracionMatricula)
        {
            IdIntegracionMatricula = idIntegracionMatricula;
        }
    }
}
