using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion
{
    public class GetInfoAlumnoByIdAlumnoIntegracionQueryHandler : IRequestHandler<GetInfoAlumnoByIdAlumnoIntegracionQuery, AlumnoInfoDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly ICommonsServiceClient _commonsServiceClient;
        private readonly IMapper _mapper;

        public GetInfoAlumnoByIdAlumnoIntegracionQueryHandler(IExpedientesContext context, IErpAcademicoServiceClient erpAcademicoServiceClient, ICommonsServiceClient commonsServiceClient, IMapper mapper)
        {
            _context = context;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _commonsServiceClient = commonsServiceClient;
            _mapper = mapper;
        }

        public async Task<AlumnoInfoDto> Handle(GetInfoAlumnoByIdAlumnoIntegracionQuery request, CancellationToken cancellationToken)
        {
            var expedientesAlumno =
                await _context.ExpedientesAlumno
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.RequisitoExpediente)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.EstadoRequisitoExpediente)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.TipoRequisitoExpediente)
                .Where(ea => ea.IdRefIntegracionAlumno == request.IdAlumnoIntegracion)
                    .ToListAsync(cancellationToken);

            var expedienteAlumno = expedientesAlumno.FirstOrDefault();
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdAlumnoIntegracion);

            var alumnoAcademico = await _erpAcademicoServiceClient
                .GetAlumnoMatriculasDocumentos(int.Parse(request.IdAlumnoIntegracion));

            var universidad =
                await _erpAcademicoServiceClient.GetUniversidadById(int.Parse(expedienteAlumno.IdRefUniversidad));

            var fotoAlumnoAcademico = await _erpAcademicoServiceClient.GetFotoAlumnoById(alumnoAcademico.Id);
            alumnoAcademico.Persona.Foto = fotoAlumnoAcademico;

            return await GetAlumnoInfo(expedienteAlumno, alumnoAcademico, universidad, expedientesAlumno);
        }

        protected internal virtual async Task<AlumnoInfoDto> GetAlumnoInfo(ExpedienteAlumno expedienteAlumno, 
            AlumnoAcademicoModel alumnoAcademico, UniversidadAcademicoModel universidad, List<ExpedienteAlumno> expedientes)
        {
            return new AlumnoInfoDto
            {
                IdAlumno = alumnoAcademico.Id,
                IdIntegracionAlumno = expedienteAlumno.IdRefIntegracionAlumno,
                DisplayName =
                    $"{expedienteAlumno.AlumnoNombre} {expedienteAlumno.AlumnoApellido1} {expedienteAlumno.AlumnoApellido2}",
                Foto = alumnoAcademico.Persona.Foto,
                Sexo = alumnoAcademico.Persona.Sexo,
                Celular = alumnoAcademico.Persona.Celular,
                Nacionalidad = alumnoAcademico.Persona.IdRefPaisNacionalidad != null
                    ? (await _commonsServiceClient.GetCountry(alumnoAcademico.Persona.IdRefPaisNacionalidad))?.Name
                    : null,
                FechaNacimiento = alumnoAcademico.Persona.FechaNacimiento,
                Email = expedienteAlumno.AlumnoEmail,
                TipoDocumentoIdentificacionPais = expedienteAlumno.IdRefTipoDocumentoIdentificacionPais,
                NroDocIdentificacion = expedienteAlumno.AlumnoNroDocIdentificacion,
                DocumentosIdentificacion = alumnoAcademico.Persona.DocumentosIdentificacion,
                DocumentosAlumno = alumnoAcademico.DocumentosAlumnos,
                AcronimoUniversidad = expedienteAlumno.AcronimoUniversidad,
                IdUniversidadIntegracion = universidad.IdIntegracion,
                Expedientes = _mapper.Map<List<ExpedienteAlumno>, List<ExpedienteDto>>(expedientes),
                Matriculas = alumnoAcademico.Matriculas?.Select(m => new MatriculaDto
                {
                    Id = m.Id,
                    IdIntegracion = m.IdIntegracion,
                    DisplayName = m.DisplayName,
                    IdRefExpedienteAlumno = m.IdRefExpedienteAlumno,
                    TotalCreditosAsignaturasMatriculadasActivas = m.TotalCreditosAsignaturasMatriculadasActivas,
                    Tipo = new TipoMatriculaDto
                    {
                        DisplayName = m.Tipo.DisplayName
                    },
                    RegionEstudio = new RegionEstudioDto
                    {
                        DisplayName = m.RegionEstudio?.DisplayName
                    },
                    Estado = new EstadoMatriculaDto
                    {
                        DisplayName = m.Estado.DisplayName
                    },
                    PlanOfertado = new PlanOfertadoDtoDto
                    {
                        PeriodoAcademico = new PeriodoAcademicoDto
                        {
                            DisplayName = m.PlanOfertado.PeriodoAcademico.DisplayName,
                            FechaInicio = m.PlanOfertado.PeriodoAcademico.FechaInicio,
                            AnyoAcademico = new AnyoAcademicoDto
                            {
                                DisplayName = m.PlanOfertado.PeriodoAcademico.AnyoAcademico.DisplayName
                            }
                        },
                        Plan = new PlanDto
                        {
                            DisplayName = m.PlanOfertado.Plan.DisplayName
                        }
                    }
                }).ToList()
            };
        }
    }
}
