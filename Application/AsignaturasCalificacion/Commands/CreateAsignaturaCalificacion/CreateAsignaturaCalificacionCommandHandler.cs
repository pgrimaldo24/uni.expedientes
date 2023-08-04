using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.NotaFinalGenerada;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion
{
    public class CreateAsignaturaCalificacionCommandHandler : IRequestHandler<CreateAsignaturaCalificacionCommand>
    {
        private readonly IMediator _mediator;
        private readonly IExpedientesContext _context;

        public CreateAsignaturaCalificacionCommandHandler(IMediator mediator, IExpedientesContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task<Unit> Handle(CreateAsignaturaCalificacionCommand request, CancellationToken cancellationToken)
        {
            
            foreach (var nota in request.notaFinal.Notas)
            {
                var asignaturaCalificacion = new AsignaturaCalificacion();
                var asignaturaMatriculada = request.asignaturasMatriculadas.FirstOrDefault(am =>
                    am.Matricula.Alumno.IdIntegracion == nota.IdAlumno.ToString());
                if (asignaturaMatriculada is null)
                    continue;
                var expediente = request.Expedientes.FirstOrDefault(e => e.IdRefIntegracionAlumno == nota.IdAlumno.ToString());
                if (expediente is null)
                    continue;
                var asignaturaExpediente = expediente.AsignaturasExpedientes.FirstOrDefault(ae =>
                    ae.IdRefAsignaturaPlan ==
                    asignaturaMatriculada.AsignaturaOfertada.AsignaturaPlan.Id.ToString());
                if (asignaturaExpediente is null)
                    continue;
                var configuracionVersionEscala = asignaturaMatriculada.ConfiguracionVersionEscala;
                if (configuracionVersionEscala is null)
                    continue;
                var tipoConvocatoria =
                    await _context.TiposConvocatorias.FirstOrDefaultAsync(tc => tc.Codigo.ToUpper() == nota.Convocatoria.ToUpper() || tc.Nombre.ToUpper() == nota.Convocatoria.ToUpper(),
                        cancellationToken);
                AssignAsignaturaCalificacion(nota, asignaturaMatriculada,
                    asignaturaExpediente, configuracionVersionEscala, asignaturaCalificacion, tipoConvocatoria,
                    request.notaFinal, cancellationToken);

                await _context.AsignaturasCalificaciones.AddAsync(asignaturaCalificacion, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                await _mediator.Send(new EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand
                {
                    IdAsignaturaExpedienteAlumno = asignaturaExpediente.Id,
                    EsSuperada = asignaturaCalificacion.Superada,
                    EsMatriculaHonor = asignaturaCalificacion.EsMatriculaHonor,
                    EsNoPresentado = asignaturaCalificacion.EsNoPresentado
                });
            }
            return Unit.Value;
        }

        protected internal virtual void AssignAsignaturaCalificacion(NotaCommonStruct nota,
            AsignaturaMatriculadaModel asignaturaMatriculada, AsignaturaExpedienteDto asignaturaExpediente,
            ConfiguracionVersionEscalaModel configuracionVersionEscala,
            AsignaturaCalificacion newAsignaturaCalificacion, TipoConvocatoria tipoConvocatoria,
            NotaFinalGeneradaCommand notaFinal, CancellationToken cancellationToken)
        {
            newAsignaturaCalificacion.FechaConfirmado = nota.FechaConfirmado;
            newAsignaturaCalificacion.FechaPublicado =
                nota.FechaPublicado.HasValue ? nota.FechaPublicado.Value : DateTime.UtcNow;
            newAsignaturaCalificacion.IdRefPeriodoLectivo = asignaturaMatriculada.AsignaturaOfertada.PeriodoLectivo.Id;
            newAsignaturaCalificacion.Ciclo = (nota.FechaConfirmado.HasValue ? nota.FechaConfirmado.Value.Year.ToString() : newAsignaturaCalificacion.FechaPublicado?.Year.ToString())  + "-" +
                                              CalculoCiclo((nota.FechaConfirmado.HasValue ? nota.FechaConfirmado.Value : newAsignaturaCalificacion.FechaPublicado),
                                                  asignaturaMatriculada.AsignaturaOfertada.PeriodoLectivo
                                                      .DuracionPeriodoLectivo.Simbolo)
                ;
            newAsignaturaCalificacion.AnyoAcademico =
                asignaturaMatriculada.AsignaturaOfertada.PeriodoLectivo.PeriodoAcademico.AnyoAcademico.AnyoInicio
                    .ToString() + "-" + asignaturaMatriculada.AsignaturaOfertada.PeriodoLectivo.PeriodoAcademico
                    .AnyoAcademico.AnyoFin.ToString();
            newAsignaturaCalificacion.Calificacion = (decimal?)nota.Calificacion;
            newAsignaturaCalificacion.NombreCalificacion = GetNombreCalificacion(nota, configuracionVersionEscala);
            newAsignaturaCalificacion.Convocatoria = CalcularNumeroConvocatoria(asignaturaExpediente, tipoConvocatoria);
            newAsignaturaCalificacion.IdRefAsignaturaMatriculada = asignaturaMatriculada.Id;
            newAsignaturaCalificacion.IdRefAsignaturaOfertada = asignaturaMatriculada.AsignaturaOfertada.Id;
            newAsignaturaCalificacion.Plataforma = notaFinal.Plataforma;
            newAsignaturaCalificacion.IdRefGrupoCurso = Convert.ToInt32(asignaturaMatriculada.IdRefCurso);
            newAsignaturaCalificacion.IdPublicadorConfirmador = notaFinal.IdUsuarioPublicadorConfirmador;
            newAsignaturaCalificacion.EsMatriculaHonor = nota.EsMatriculaHonor;
            newAsignaturaCalificacion.EsNoPresentado = nota.NoPresentado;
            newAsignaturaCalificacion.Superada = !nota.NoPresentado &&
                                                 (nota.Calificacion >= configuracionVersionEscala.Configuracion
                                                     .Calificacion.NotaMinAprobado);
            newAsignaturaCalificacion.TipoConvocatoria = tipoConvocatoria;
            newAsignaturaCalificacion.TipoConvocatoriaId = tipoConvocatoria.Id;
            newAsignaturaCalificacion.EstadoCalificacionId = notaFinal.Provisional
                ? EstadoCalificacion.EstadoProvisional
                : EstadoCalificacion.EstadoDefinitiva;
            newAsignaturaCalificacion.AsignaturaExpedienteId = asignaturaExpediente.Id;
        }

        protected internal virtual string CalculoCiclo(DateTime? fecha, string simbolo)
        {
            const int bimestral = 2;
            const int trimestral = 3;
            const int cuatrimestral = 4;
            const int semestral = 6;
            const string anual = "1";

            if (!fecha.HasValue)
                return "";

            switch (simbolo)
            {
                case "M":
                    return fecha.Value.Month.ToString();
                case "B":
                    return (fecha.Value.Month % bimestral) > 0? Convert.ToInt32((fecha.Value.Month / bimestral) + 1).ToString() : Convert.ToInt32(fecha.Value.Month / bimestral).ToString();
                case "T":
                    return (fecha.Value.Month % trimestral) > 0 ? Convert.ToInt32((fecha.Value.Month / trimestral) + 1).ToString() : Convert.ToInt32(fecha.Value.Month / trimestral).ToString();
                case "C":
                    return (fecha.Value.Month % cuatrimestral) > 0 ? Convert.ToInt32((fecha.Value.Month / cuatrimestral) + 1).ToString() : Convert.ToInt32(fecha.Value.Month / cuatrimestral).ToString();
                case "S":
                    return (fecha.Value.Month % semestral) > 0 ? Convert.ToInt32((fecha.Value.Month / semestral) + 1).ToString() : Convert.ToInt32(fecha.Value.Month / semestral).ToString();
                case "A":
                    return anual;
                default:
                    return fecha.Value.Month.ToString();
            }
        }

        protected internal virtual string GetNombreCalificacion(NotaCommonStruct nota, ConfiguracionVersionEscalaModel configuracionVersionEscala)
        {
            if (nota.NoPresentado)
            {
                return configuracionVersionEscala.Configuracion.Calificacion.Calificaciones.First(c => c.EsNoPresentado)
                    .Nombre;
            }

            if (configuracionVersionEscala.Configuracion.Calificacion.AfectaPorcentaje)
            {
                return configuracionVersionEscala.Configuracion.Calificacion.Calificaciones
                    .Where(c => c.PorcentajeMinimo != null && c.PorcentajeMinimo.Value <= nota.Calificacion).OrderByDescending(c => c.PorcentajeMinimo).FirstOrDefault()
                    ?.Nombre;
            }

            return configuracionVersionEscala.Configuracion.Calificacion.Calificaciones
                .Where(c => c.NotaMinima != null && c.NotaMinima.Value <= nota.Calificacion).OrderByDescending(c => c.NotaMinima).FirstOrDefault()
                ?.Nombre;
        }

        protected internal virtual int CalcularNumeroConvocatoria(AsignaturaExpedienteDto asignaturaExpediente, 
            TipoConvocatoria tipoConvocatoria)
        {
            return _context.AsignaturasCalificaciones.Where(ac =>
                    ac.TipoConvocatoria.Id == tipoConvocatoria.Id &&
                    ac.AsignaturaExpediente.Id == asignaturaExpediente.Id)
                .ToList().Count + 1;
        }
    }
}
