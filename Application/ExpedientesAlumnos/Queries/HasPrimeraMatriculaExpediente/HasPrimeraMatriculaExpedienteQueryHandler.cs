using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente
{
    public class HasPrimeraMatriculaExpedienteQueryHandler : IRequestHandler<HasPrimeraMatriculaExpedienteQuery, bool>
    {
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        public HasPrimeraMatriculaExpedienteQueryHandler(
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<bool> Handle(HasPrimeraMatriculaExpedienteQuery request, CancellationToken cancellationToken)
        {
            var matricula = await _erpAcademicoServiceClient
                .GetMatriculaByIdIntegracionMatricula(request.IdIntegracionMatricula);

            return matricula != null && matricula.Tipo.PermiteCrearPrimeraMatricula
                    && !matricula.Estado.EsBaja && !matricula.Estado.EsDesestimada;
        }
    }
}
