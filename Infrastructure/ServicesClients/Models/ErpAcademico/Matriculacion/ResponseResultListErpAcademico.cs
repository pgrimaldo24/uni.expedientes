using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseResultListErpAcademico<T>
    {
        private List<T> _elements;

        public List<T> Elements
        {
            get
            {
                return _elements;
            }
            set
            {
                _elements = value;
            }
        }
    }
}
