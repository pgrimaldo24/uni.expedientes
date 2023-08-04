using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.GestorDocumental;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IGestorDocumentalServiceClient
    {
        Task<DocumentoModel> SaveDocumento(DocumentoParameters documento);
        Task<List<ClasificacionModel>> GetClasificaciones(ClasificacionListParameters clasificacionParameters);
    }
}
