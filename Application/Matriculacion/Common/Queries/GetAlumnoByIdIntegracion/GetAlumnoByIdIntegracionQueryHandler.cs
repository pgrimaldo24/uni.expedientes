using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion
{
    public class GetAlumnoByIdIntegracionQueryHandler : IRequestHandler<GetAlumnoByIdIntegracionQuery, AlumnoMatricula>
    {
        private readonly IExpedientesContext _context;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler> _localizer;
        public GetAlumnoByIdIntegracionQueryHandler(
            IExpedientesContext context,
            IStringLocalizer<GetAlumnoByIdIntegracionQueryHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<AlumnoMatricula> Handle(GetAlumnoByIdIntegracionQuery request, CancellationToken cancellationToken)
        {
            var alumnoAcademico = await _erpAcademicoServiceClient
                .GetAlumnoMatriculasDocumentos(Convert.ToInt32(request.AlumnoIdIntegracion));

            if(alumnoAcademico == null)
                throw new BadRequestException(_localizer["El Alumno no existe en erp académico"]);

            var matriculaActual = alumnoAcademico.Matriculas.FirstOrDefault(m => m.IdIntegracion == request.MatriculaIdIntegracion);
            if (matriculaActual == null)
                throw new BadRequestException(_localizer["La matrícula no existe"]);

            var expedienteAlumno = await _context.ExpedientesAlumno
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .Include(ts => ts.TiposSituacionEstadoExpedientes)
                .Include(hc => hc.HitosConseguidos)
                .Include(ae => ae.AsignaturasExpedientes)
                .FirstOrDefaultAsync(ea => ea.Id == Convert.ToInt32(matriculaActual.IdRefExpedienteAlumno), cancellationToken);
            if (expedienteAlumno == null)
                throw new BadRequestException(_localizer["El expediente de alumno no existe"]);

            return new AlumnoMatricula
            {
                ExpedienteAlumno = expedienteAlumno,
                AlumnoAcademicoModel = alumnoAcademico,
                MatriculaAcademicoModel = matriculaActual
            };
        }
    }
}
