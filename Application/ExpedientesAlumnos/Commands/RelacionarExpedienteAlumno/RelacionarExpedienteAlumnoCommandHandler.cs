using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno
{
    public class RelacionarExpedienteAlumnoCommandHandler : IRequestHandler<RelacionarExpedienteAlumnoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public RelacionarExpedienteAlumnoCommandHandler(IExpedientesContext context,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<Unit> Handle(RelacionarExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var idsPlanesARelacionar = await _erpAcademicoServiceClient.GetIdsPlanesARelacionar(
                    Convert.ToInt32(request.ExpedienteAlumno.IdRefPlan), Convert.ToInt32(request.ExpedienteAlumno.IdRefEstudio));
            if (!idsPlanesARelacionar.Any()) return Unit.Value;

            await AssignExpedienteAlumnoRelacionado(idsPlanesARelacionar, request, cancellationToken);
            return Unit.Value;
        }

        protected internal virtual async Task AssignExpedienteAlumnoRelacionado(List<int> idsPlanes, 
            RelacionarExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = request.ExpedienteAlumno;
            var expedientesAlumnos = await _context.ExpedientesAlumno.Where(e =>
                                                e.IdRefIntegracionAlumno == expedienteAlumno.IdRefIntegracionAlumno
                                                && idsPlanes.Contains(Convert.ToInt32(e.IdRefPlan))).ToListAsync(cancellationToken);
            if (!expedientesAlumnos.Any()) return;

            foreach (var idPlan in idsPlanes)
            {
                var expedienteRelacionado = expedientesAlumnos.FirstOrDefault(e => e.IdRefPlan == idPlan.ToString());
                if (expedienteRelacionado == null) continue;

                await _context.RelacionesExpedientes.AddAsync(new RelacionExpediente
                {
                    ExpedienteAlumnoRelacionado = expedienteRelacionado,
                    TipoRelacion = request.TipoRelacionExpediente,
                    ExpedienteAlumno = expedienteAlumno
                }, cancellationToken);

                var descripcionSeguimiento = "Actualizado: Relación entre expedientes";
                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizadoEnProcesoMasivo, descripcionSeguimiento);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }
        }
    }
}