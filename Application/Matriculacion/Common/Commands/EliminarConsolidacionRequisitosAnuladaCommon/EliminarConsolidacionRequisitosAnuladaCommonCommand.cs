using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon
{
    public class EliminarConsolidacionRequisitosAnuladaCommonCommand : IRequest
    {
        public List<int> IdsAsignaturasOfertadas { get; set; }
        public AlumnoMatricula Alumno { get; set; }

        public EliminarConsolidacionRequisitosAnuladaCommonCommand(List<int> idsAsignaturasOfertadas, AlumnoMatricula alumno)
        {
            IdsAsignaturasOfertadas = idsAsignaturasOfertadas;
            Alumno = alumno;
        }
    }
}
