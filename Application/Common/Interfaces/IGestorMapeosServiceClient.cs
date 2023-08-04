using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IGestorMapeosServiceClient
    {
        Task<List<EstudioGestorMapeoModel>> GetEstudios(string idRefPlan, 
            string idRefVersionPlan, int? idEstudioGestor = null);
        Task<List<AsignaturaGestorMapeoModel>> GetAsignaturas(int? idEstudioGestor = null, 
            int? idAsignaturaEstudioGestor = null);
        Task<AsignaturaIntegrationGestorMapeoModel> GetAsignatura(int idAsignaturaEstudioGestor);
        Task<EstudioIntegrationGestorMapeoModel> GetEstudio(int idEstudioGestor);
    }
}
