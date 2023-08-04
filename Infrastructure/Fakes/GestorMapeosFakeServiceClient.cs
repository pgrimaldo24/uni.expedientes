using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.GestorMapeos;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class GestorMapeosFakeServiceClient: IGestorMapeosServiceClient
    {
        private readonly IIntegrationFakeServices _integrationFakeServices;

        public GestorMapeosFakeServiceClient(IIntegrationFakeServices integrationFakeServices)
        {
            _integrationFakeServices = integrationFakeServices;
        }

        public Task<AsignaturaIntegrationGestorMapeoModel> GetAsignatura(int idAsignaturaEstudioGestor)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaGestorMapeoModel>> GetAsignaturas(int? idEstudioGestor = null, 
            int? idAsignaturaEstudioGestor = null)
        {
            throw new NotImplementedException();
        }

        public Task<EstudioIntegrationGestorMapeoModel> GetEstudio(int idEstudioGestor)
        {
            throw new NotImplementedException();
        }

        public Task<List<EstudioGestorMapeoModel>> GetEstudios(string idRefPlan, 
            string idRefVersionPlan, int? idEstudioGestor = null)
        {
            throw new NotImplementedException();
        }
    }
}
