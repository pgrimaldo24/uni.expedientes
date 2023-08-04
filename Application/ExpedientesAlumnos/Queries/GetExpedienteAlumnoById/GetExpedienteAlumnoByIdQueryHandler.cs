using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById
{
    public class GetExpedienteAlumnoByIdQueryHandler : IRequestHandler<GetExpedienteAlumnoByIdQuery, ExpedienteAlumnoItemDto>
    {
        private const int Index = 1;
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly ICommonsServiceClient _commonsServiceClient;

        public GetExpedienteAlumnoByIdQueryHandler(
            IExpedientesContext context, 
            IMapper mapper,
            IErpAcademicoServiceClient erpAcademicoServiceClient,
            ICommonsServiceClient commonsServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _commonsServiceClient = commonsServiceClient;
        }

        public async Task<ExpedienteAlumnoItemDto> Handle(GetExpedienteAlumnoByIdQuery request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno
                .Include(ea => ea.TitulacionAcceso)
                .Include(ea => ea.ExpedientesEspecializaciones)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.RequisitoExpediente)
                .ThenInclude(re => re.RequisitosExpedientesDocumentos)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.RequisitoExpediente)
                .ThenInclude(cre => cre.RolesRequisitosExpedientes)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.EstadoRequisitoExpediente)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .ThenInclude(cre => cre.TipoRequisitoExpediente)
                .Include(ea => ea.Estado)
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var expedienteAlumnoItemDto = _mapper.Map<ExpedienteAlumno, ExpedienteAlumnoItemDto>(expedienteAlumno);
            var alumnoAcademico = await _erpAcademicoServiceClient
                    .GetAlumnoMatriculasDocumentos(int.Parse(expedienteAlumno.IdRefIntegracionAlumno));

            alumnoAcademico.Persona.Foto = await _erpAcademicoServiceClient.GetFotoAlumnoById(alumnoAcademico.Id);
            expedienteAlumnoItemDto.Alumno = await GetAlumnoInfo(expedienteAlumno, alumnoAcademico);
            expedienteAlumnoItemDto.ExpedientesEspecializaciones = await GetEspecializacionesDisplayName(expedienteAlumno.ExpedientesEspecializaciones.ToList());
            if (expedienteAlumnoItemDto.TitulacionAcceso == null)
                return expedienteAlumnoItemDto;

            expedienteAlumnoItemDto.TitulacionAcceso.CodeCountryInstitucionDocente =
                    (!string.IsNullOrEmpty(expedienteAlumnoItemDto.TitulacionAcceso.IdRefInstitucionDocente) && expedienteAlumnoItemDto.TitulacionAcceso.IdRefInstitucionDocente != "-1")
                        ? expedienteAlumnoItemDto.TitulacionAcceso.IdRefInstitucionDocente.Split("-")[0]
                        : !string.IsNullOrEmpty(expedienteAlumnoItemDto.TitulacionAcceso.IdRefTerritorioInstitucionDocente)
                        ? expedienteAlumnoItemDto.TitulacionAcceso.IdRefTerritorioInstitucionDocente.Split("-")[0]
                        : string.Empty;
            return expedienteAlumnoItemDto;
        }

        protected internal virtual async Task<List<ExpedienteEspecializacionDto>> GetEspecializacionesDisplayName(List<ExpedienteEspecializacion> expedienteEspecializacion)
        {
            var expedienteEspecializacionDtos = new List<ExpedienteEspecializacionDto>();
            if (expedienteEspecializacion == null || !expedienteEspecializacion.Any())
                return expedienteEspecializacionDtos;

            var especializacionesErp =
                await _erpAcademicoServiceClient.GetEspecializaciones(new EspecializacionListParameters
                {
                    Count = expedienteEspecializacion.Count,
                    Index = Index,
                    FilterIdsEspecializaciones = expedienteEspecializacion.Select(x => x.IdRefEspecializacion).ToArray()
                });
                
            especializacionesErp.ForEach(e =>
            {
                expedienteEspecializacionDtos.Add(new ExpedienteEspecializacionDto
                {
                    Id = e.Id,
                    DisplayName = e.DisplayName
                });
            });

            return expedienteEspecializacionDtos;
        }

        protected internal virtual async Task<AlumnoDto> GetAlumnoInfo(
            ExpedienteAlumno expedienteAlumno, AlumnoAcademicoModel alumnoAcademico)
        {
            var universidad =
                await _erpAcademicoServiceClient.GetUniversidadById(int.Parse(expedienteAlumno.IdRefUniversidad));
            return new AlumnoDto
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
                IdUniversidadIntegracion = universidad.IdIntegracion
            };
        }
    }
}
