using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IExpedicionTitulosServiceClient
    {
        Task<List<SolicitudExpedicionTitulosModel>> GetSolicitudes(int idIntegracionAlumno, int idPlan, bool excluirCanceladas);
        Task<List<TiposSolicitudesTitulosModel>> GetTiposSolicitudes(TiposSolicitudesParameters tiposSolicitudesParameters);
        Task<TipoSolicitudTituloModel> GetTipoSolicitudTituloById(int id);
    }
}
