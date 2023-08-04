using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit
{
    public class AddSeguimientoTitulacionAccesoUncommitCommand : IRequest<bool>
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public bool PorIntegracion { get; set; }
        public TitulacionAccesoParametersDto TitulacionAcceso { get; set; }

        public AddSeguimientoTitulacionAccesoUncommitCommand(ExpedienteAlumno expedienteAlumno, bool porIntegracion,
            TitulacionAccesoParametersDto titulacionAcceso)
        {
            ExpedienteAlumno = expedienteAlumno;
            PorIntegracion = porIntegracion;
            TitulacionAcceso = titulacionAcceso;
        }
    }
}
