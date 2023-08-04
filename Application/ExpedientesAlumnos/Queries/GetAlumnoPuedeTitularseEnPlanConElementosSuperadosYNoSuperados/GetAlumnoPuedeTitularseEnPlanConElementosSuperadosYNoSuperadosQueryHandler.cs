using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados
{
    public class GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler : IRequestHandler<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery, AlumnoPuedeTitularseDto>
    {
        private const int AsignaturaReconocidaTipoSeminario1 = -1;
        private const int AsignaturaReconocidaTipoSeminario4 = -4;
        private const int AsignaturaReconocidaTipoReconocimiento = -2;
        private const int AsignaturaReconocidaTipoActividades = -3;
        private const int AsignaturaReconocidaTipoSeminario9 = 999999;
        private const string TipoReconocimiento = "Reconocimiento";

        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler> _localizer;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler(IExpedientesContext context,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient,
            IStringLocalizer<GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQueryHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<AlumnoPuedeTitularseDto> Handle(GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery request,
            CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno.FirstOrDefaultAsync(
                        ea => ea.IdRefIntegracionAlumno == request.IdRefIntegracionAlumno && ea.IdRefPlan == request.IdRefPlan,
                        cancellationToken);
            if (expedienteAlumno == null)
                throw new BadRequestException(_localizer[
                    $"No se ha encontrado un Expediente con el IdPlan {request.IdRefPlan} y IdIntegracionAlumno {request.IdRefIntegracionAlumno}."]);

            var resultExpedienteGestor =
                await _expedientesGestorUnirServiceClient.GetExpedienteGestorFormatoErpWithAsignaturas(
                    request.IdRefIntegracionAlumno, int.Parse(request.IdRefPlan));
            if (resultExpedienteGestor.HasErrors)
                throw new BadRequestException(_localizer[$"[Expedientes Gestor]: {string.Join(", ", resultExpedienteGestor.Errors)}."]);

            var expedienteGestor = resultExpedienteGestor.Value;
            if (expedienteGestor?.Asignaturas == null || !expedienteGestor.Asignaturas.Any())
                throw new BadRequestException(_localizer["No se puede localizar las Asignaturas en el Expediente del Gestor Unir."]);

            RemoveAsignaturasGestorDuplicadas(expedienteGestor.Asignaturas);

            var resultIsPlanSuperado = await GetAsignaturasPlanes(expedienteAlumno, expedienteGestor);
            var planSuperadoExpedienteAlumno = resultIsPlanSuperado;
            var idsAsignatura = expedienteGestor.Asignaturas.Select(a => a.IdAsignatura).ToArray();

            var filterAsignaturasPlanTitulacionParameters = new AsignaturasPlanTitulacionParameters
            {
                IdIntegracionExpediente = expedienteAlumno.Id.ToString(),
                IdRefUniversidad = request.IdRefUniversidad,
                IdsAsignaturaPlan = idsAsignatura
            };
            var datosAdicionalesAsignaturaPlan = await _erpAcademicoServiceClient.GetAsignaturasPlanesParaTitulacion(filterAsignaturasPlanTitulacionParameters);
            planSuperadoExpedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.ForEach(ap =>
                SetAsignaturasConIntegracionErpAcademico(ap, datosAdicionalesAsignaturaPlan,
                    expedienteGestor.Asignaturas));
            planSuperadoExpedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.ForEach(ap =>
                SetAsignaturasConIntegracionErpAcademico(ap, datosAdicionalesAsignaturaPlan,
                    expedienteGestor.Asignaturas));
            await SetExpedicion(planSuperadoExpedienteAlumno, expedienteGestor, int.Parse(request.IdRefPlan));
            SetCreditosObtenidosAsignaturasSuperadas(planSuperadoExpedienteAlumno);
            SetArcosSuperados(planSuperadoExpedienteAlumno);
            planSuperadoExpedienteAlumno.Bloqueado = expedienteGestor.Bloqueado;
            if (expedienteGestor.Bloqueado)
            {
                SetBloqueos(planSuperadoExpedienteAlumno, expedienteGestor.Bloqueos);
            }

            var resultCausasFalloMatriculas = await
                GetCausasFallosComprobacionMatriculasDocumentacionErp(expedienteAlumno.IdRefIntegracionAlumno, expedienteAlumno.IdRefPlan);

            planSuperadoExpedienteAlumno.MatriculasOk = resultCausasFalloMatriculas.MatriculasOk;
            planSuperadoExpedienteAlumno.CausasFalloMatriculas = resultCausasFalloMatriculas.CausasFallosMatriculas;
            planSuperadoExpedienteAlumno.PuedeTitular = planSuperadoExpedienteAlumno.MatriculasOk && planSuperadoExpedienteAlumno.EsPlanSuperado.EsSuperado;

            planSuperadoExpedienteAlumno.Errores.AddRange(planSuperadoExpedienteAlumno.EsPlanSuperado.ElementosSuperados
                .AsignaturasPlanSuperadas.SelectMany(a => a.Asignatura.DatosGestor.ErroresReconocimientosOrigen)
                .ToArray());
            planSuperadoExpedienteAlumno.Errores.AddRange(planSuperadoExpedienteAlumno.ElementosNoSuperados
                .AsignaturasPlanNoSuperadas.SelectMany(a => a.Asignatura.DatosGestor.ErroresReconocimientosOrigen)
                .ToArray());

            FiltrarAsignaturasExpedienteAlumno(planSuperadoExpedienteAlumno, request);
            return planSuperadoExpedienteAlumno;
        }

        protected internal virtual void RemoveAsignaturasGestorDuplicadas(
            List<AsignaturaErpAcademicoExpedientesIntegrationModel> asignaturasGestor)
        {
            var idsAsignaturasRepetidas = asignaturasGestor.Select(x => x.IdAsignatura)
                                            .GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
            if (!idsAsignaturasRepetidas.Any()) return;
            foreach (var idAsignaturaRepetida in idsAsignaturasRepetidas)
            {
                var asignaturaValida = GetAsignaturaGestor(idAsignaturaRepetida, asignaturasGestor);
                var listaHashCodes = asignaturasGestor.Where(ae => ae.IdAsignatura == idAsignaturaRepetida).Select(hc => hc.GetHashCode());
                var hashCodesAEliminar = listaHashCodes.Where(hashCode => hashCode != asignaturaValida.GetHashCode()).ToList();
                asignaturasGestor.RemoveAll(ae => hashCodesAEliminar.Contains(ae.GetHashCode()));
            }
        }

        protected internal virtual async Task<AlumnoPuedeTitularseDto> GetAsignaturasPlanes(
            ExpedienteAlumno expedienteAlumno,
            ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel)
        {
            var result = new AlumnoPuedeTitularseDto();

            if (expedienteExpedientesIntegrationModel.Asignaturas == null ||
                !expedienteExpedientesIntegrationModel.Asignaturas.Any())
            {
                const string causaInsuperacionAsignaturasSuperadas = "El alumno no tiene ninguna Asignatura Superada.";
                result.EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    CausasInsuperacion = new List<string>
                    {
                        causaInsuperacionAsignaturasSuperadas
                    }
                };
                return result;
            }

            var asignaturasExpedientes = expedienteExpedientesIntegrationModel.Asignaturas.ToList();
            var asignaturasReconocidas = asignaturasExpedientes.Where(a => a.GetAsignaturasReconocidas()).ToList();
            RemoverAsignaturasReconocidas(asignaturasExpedientes);
            var resultPlanSuperado = await GetPlanSurpassedErp(int.Parse(expedienteAlumno.IdRefPlan), expedienteAlumno, asignaturasExpedientes);
            var resultPlanSuperadoExpedienteAlumno = ConvertToExpedienteAlumno(resultPlanSuperado);
            SetAsignaturasReconocidas(resultPlanSuperadoExpedienteAlumno, asignaturasReconocidas);

            var idsAsignaturasNoSuperadas = asignaturasExpedientes.Where(a => !a.Superado).Select(a => a.IdAsignatura).ToArray();
            var asignaturasBloqueNoSuperadas = resultPlanSuperadoExpedienteAlumno.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                .SelectMany(ab => ab.AsignaturasBloqueSuperadas)
                .Where(a => idsAsignaturasNoSuperadas.Contains(a.IdAsignaturaPlan)).ToList();
            SetElementosNoSuperados(idsAsignaturasNoSuperadas, asignaturasBloqueNoSuperadas, resultPlanSuperadoExpedienteAlumno);
            RemoverAsignaturasNoSuperadas(idsAsignaturasNoSuperadas, resultPlanSuperadoExpedienteAlumno);
            RemoverBloquesAsignaturasNoSuperadas(asignaturasBloqueNoSuperadas, resultPlanSuperadoExpedienteAlumno);
            RemoverSubBloquesAsignaturasNoSuperadas(idsAsignaturasNoSuperadas, resultPlanSuperadoExpedienteAlumno);
            return resultPlanSuperadoExpedienteAlumno;
        }

        protected internal virtual void RemoverAsignaturasReconocidas(List<AsignaturaErpAcademicoExpedientesIntegrationModel> asignaturasSuperadas)
        {
            var asignaturasReconocidas = asignaturasSuperadas.Where(a => a.GetAsignaturasReconocidas()).ToList();
            foreach (var asignatura in asignaturasReconocidas)
            {
                asignaturasSuperadas.RemoveAll(a => a.IdAsignatura == asignatura.IdAsignatura);
            }
        }

        protected internal virtual async Task<ExpedienteAlumnoTitulacionPlanDto> GetPlanSurpassedErp(int idPlan,
            ExpedienteAlumno expedienteAlumno,
            List<AsignaturaErpAcademicoExpedientesIntegrationModel> asignaturasSuperadas)
        {
            var filtersPlanSuperadoParameters = new EsPlanSuperadoParameters
            {
                IdNodo = int.Parse(expedienteAlumno.IdRefNodo),
                IdVersionPlan = !string.IsNullOrWhiteSpace(expedienteAlumno.IdRefVersionPlan)
                    ? int.Parse(expedienteAlumno.IdRefVersionPlan)
                    : null,
                IdsAsignaturasPlanes = asignaturasSuperadas.Select(a => a.IdAsignatura).ToList()
            };

            var resultPlanSuperado = await
                _erpAcademicoServiceClient.ItIsPlanSurpassed(idPlan, filtersPlanSuperadoParameters);
            return new ExpedienteAlumnoTitulacionPlanDto
            {
                EsPlanSuperado = resultPlanSuperado
            };
        }

        #region Conversión a modelo AlumnoPuedeTitularse

        protected internal virtual AlumnoPuedeTitularseDto ConvertToExpedienteAlumno(ExpedienteAlumnoTitulacionPlanDto resultPlanSuperado)
        {
            var result = new AlumnoPuedeTitularseDto
            {
                EsPlanSuperado = new PlanSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    EsSuperado = resultPlanSuperado.EsPlanSuperado.EsSuperado,
                    CausasInsuperacion = resultPlanSuperado.EsPlanSuperado.CausasInsuperacion,
                    ElementosSuperados = new ElementoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                    {
                        AsignaturasPlanSuperadas = ConvertToAsignaturasPlanSuperadas(resultPlanSuperado.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas),
                        Arcos = ConvertToArcos(resultPlanSuperado.EsPlanSuperado.ElementosSuperados.ArcosSuperados),
                        BloquesSuperados = ConvertToBloquesSuperados(resultPlanSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados),
                        HitosObtenidos = resultPlanSuperado.EsPlanSuperado.ElementosSuperados.HitosObtenidos,
                        NodosActuales = resultPlanSuperado.EsPlanSuperado.ElementosSuperados.NodosActuales,
                        NodosAlcanzados = resultPlanSuperado.EsPlanSuperado.ElementosSuperados.NodosAlcanzados,
                        RequerimientosSuperados = resultPlanSuperado.EsPlanSuperado.ElementosSuperados.RequerimientosSuperados,
                        TrayectosPlanSuperados = resultPlanSuperado.EsPlanSuperado.ElementosSuperados.TrayectosPlanSuperados
                    }
                }
            };

            return result;
        }

        protected internal virtual List<AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel> ConvertToAsignaturasPlanSuperadas(
            List<AsignaturaPlanErpAcademicoModel> elementosSuperadosAsignaturasPlanSuperadas)
        {
            return elementosSuperadosAsignaturasPlanSuperadas.Select(asignaturaPlanModel =>
                    new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
                    {
                        Id = asignaturaPlanModel.Id,
                        Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                        {
                            Id = asignaturaPlanModel.Id,
                            Codigo = asignaturaPlanModel.Asignatura.Codigo,
                            CodigoOficialExterno = asignaturaPlanModel.Asignatura.CodigoOficialExterno,
                            Creditos = asignaturaPlanModel.Asignatura.Creditos,
                            Curso = !string.IsNullOrWhiteSpace(asignaturaPlanModel.Asignatura.Curso)
                                ? asignaturaPlanModel.Asignatura.Curso
                                : null,
                            IdentificadorOficialExterno = asignaturaPlanModel.Asignatura.IdentificadorOficialExterno,
                            Idioma = asignaturaPlanModel.Asignatura.Idioma,
                            Nombre = asignaturaPlanModel.Asignatura.Nombre,
                            Periodicidad = asignaturaPlanModel.Asignatura.Periodicidad,
                            PeriodicidadCodigoOficialExterno =
                                asignaturaPlanModel.Asignatura.PeriodicidadCodigoOficialExterno,
                            PeriodoLectivo = asignaturaPlanModel.Asignatura.PeriodoLectivo,
                            DatosGestor = ConvertToDatosGestor(asignaturaPlanModel),
                            TipoAsignatura = asignaturaPlanModel.Asignatura.TipoAsignatura
                        }
                    })
                .ToList();
        }

        protected internal virtual DatosGestorAlumnoPuedeTitularseErpAcademicoModel ConvertToDatosGestor(
            AsignaturaPlanErpAcademicoModel asignaturaPlanModel)
        {
            var result = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel();
            if (asignaturaPlanModel.Asignatura.DatosGestor != null)
            {
                result = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                {
                    Superado = asignaturaPlanModel.Asignatura.DatosGestor.Superado,
                    Ects = asignaturaPlanModel.Asignatura.DatosGestor.Ects,
                    AnyoAcademico = asignaturaPlanModel.Asignatura.DatosGestor.AnyoAcademico,
                    Convocatoria = asignaturaPlanModel.Asignatura.DatosGestor.Convocatoria,
                    IdAsignaturaGestor = asignaturaPlanModel.Asignatura.DatosGestor.IdAsignaturaGestor,
                    NotaAlfanumerica = asignaturaPlanModel.Asignatura.DatosGestor.NotaAlfanumerica,
                    NotaNumerica = asignaturaPlanModel.Asignatura.DatosGestor.NotaNumerica,
                    Observaciones = asignaturaPlanModel.Asignatura.DatosGestor.Observaciones,
                    ReconocimientosOrigen = asignaturaPlanModel.Asignatura.DatosGestor.ReconocimientosOrigen.Select(
                        ro => new ReconocimientosOrigenModel
                        {
                            Anyo = ro.Anyo,
                            Convocatoria = ro.Convocatoria,
                            Ects = ro.Ects,
                            IdAsignaturaGestor = ro.IdAsignaturaGestor,
                            NivelAprobacion = ro.NivelAprobacion,
                            NombreAsignaturaExterna = ro.NombreAsignaturaExterna,
                            NombreEstudioExterno = ro.NombreEstudioExterno,
                            Nota = ro.Nota,
                            TipoAsignaturaExterna = ro.TipoAsignaturaExterna,
                            TipoReconocimiento = ro.TipoReconocimiento
                        }).ToList()
                };
            }

            return result;
        }

        protected internal virtual List<ArcoAlumnoPuedeTitulularseErpAcademicoModel> ConvertToArcos(
            List<ArcoErpAcademicoModel> arcosSuperados)
        {
            return arcosSuperados.Select(arco => new ArcoAlumnoPuedeTitulularseErpAcademicoModel
            {
                Id = arco.Id,
                BloquesSuperados = arco.BloquesSuperados,
                Descripcion = arco.Descripcion,
                IdNodoDestino = arco.IdNodoDestino,
                IdNodoOrigen = arco.IdNodoOrigen,
                Nombre = arco.Nombre
            })
                .ToList();
        }

        protected internal virtual List<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel> ConvertToBloquesSuperados(
            List<BloqueSuperadoErpAcademicoModel> bloquesSuperados)
        {
            var result = new List<BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel>();

            foreach (var bloque in bloquesSuperados)
            {
                result.Add(new BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = bloque.Id,
                    AsignaturasBloqueSuperadas = ConvertToAsignaturasBloque(bloque.AsignaturasBloqueSuperadas),
                    SubBloquesSuperados = ConvertToSubBloques(bloque.SubBloquesSuperados),
                    CreditosMinimos = bloque.CreditosMinimos,
                    CreditosObtenidos = bloque.CreditosObtenidos,
                    Descripcion = bloque.Descripcion,
                    Nombre = bloque.Nombre
                });
            }

            return result;
        }

        protected internal virtual List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>
            ConvertToAsignaturasBloque(
                List<AsignaturaPlanBloqueErpAcademicoModel> asignaturasPlanBloque)
        {
            return asignaturasPlanBloque.Select(asignaturaPlanBloque =>
                new AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel
                { IdAsignaturaPlan = asignaturaPlanBloque.IdAsignaturaPlan }).ToList();
        }

        protected internal virtual List<SubBloqueAlumnoPuedeTitularseErpAcademicoModel> ConvertToSubBloques(
            List<SubBloqueErpAcademicoModel> subBloques)
        {
            return subBloques.Select(subBloque => new SubBloqueAlumnoPuedeTitularseErpAcademicoModel
            {
                AsignaturasSubBloqueSuperadas =
                        ConvertToAsignaturaSubBloque(subBloque.AsignaturasSubBloqueSuperadas),
                Descripcion = subBloque.Descripcion,
                CreditosMinimos = subBloque.CreditosMinimos,
                CreditosObtenidos = subBloque.CreditosObtenidos,
                Id = subBloque.Id,
                Nombre = subBloque.Nombre,
                TipoSubBloque = subBloque.TipoSubBloque
            })
                .ToList();
        }

        protected internal virtual List<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel>
            ConvertToAsignaturaSubBloque(
                List<AsignaturaPlanSubBloqueErpAcademicoModel> asignaturasSubBloqueSuperadas)
        {
            return asignaturasSubBloqueSuperadas.Select(asignaturaSubBloqueSuperadas =>
                new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                { IdAsignaturaPlan = asignaturaSubBloqueSuperadas.IdAsignaturaPlan }).ToList();
        }

        #endregion

        public virtual void SetAsignaturasReconocidas(AlumnoPuedeTitularseDto expedienteAlumnoTitulacion,
            List<AsignaturaErpAcademicoExpedientesIntegrationModel> asignaturasReconocidas)
        {
            foreach (var asignatura in asignaturasReconocidas)
            {
                if (asignatura.Superado)
                {
                    expedienteAlumnoTitulacion.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Add(
                        SetAsignaturaReconocida(asignatura));
                }
                else
                {
                    expedienteAlumnoTitulacion.ElementosNoSuperados.AsignaturasPlanNoSuperadas.Add(
                        SetAsignaturaReconocida(asignatura));
                }
            }
        }

        protected internal virtual AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel SetAsignaturaReconocida(
            AsignaturaErpAcademicoExpedientesIntegrationModel asignatura)
        {
            const int asignaturaReconocidaTipoId = 6;
            const string asignaturaReconocidaTipoNombre = "Optativas";
            const int asignaturaReconocidaTipoOrden = 4;
            const string asignaturaReconocidaTipoSimbolo = "OP";

            var asignaturaReconocida = new AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel
            {
                Id = asignatura.IdAsignatura,
                Asignatura = new AsignaturaAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = asignatura.IdAsignatura,
                    Nombre = SetNombreAsignaturaReconocida(asignatura.IdAsignatura),
                    Creditos = asignatura.Ects,
                    DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
                    {
                        IdAsignatura = asignatura.IdAsignatura.ToString(),
                        IdAsignaturaGestor = asignatura.IdAsignaturaGestor.ToString(),
                        Superado = asignatura.Superado,
                        Ects = asignatura.Ects.ToString(CultureInfo.InvariantCulture),
                        NotaNumerica = asignatura.NotaNumerica.ToString(CultureInfo.InvariantCulture),
                        NotaAlfanumerica = asignatura.NotaAlfanumerica,
                        AnyoAcademico = asignatura.AnyoAcademico,
                        Convocatoria = asignatura.Convocatoria,
                        Observaciones = asignatura.Observaciones,
                        ReconocimientosOrigen = asignatura.ReconocimientosOrigen
                    },
                    TipoAsignatura = new TipoAsignaturaErpAcademicoModel
                    {
                        Id = asignaturaReconocidaTipoId,
                        Nombre = asignaturaReconocidaTipoNombre,
                        Orden = asignaturaReconocidaTipoOrden,
                        Simbolo = asignaturaReconocidaTipoSimbolo,
                        TrabajoFinEstudio = false
                    }
                }
            };

            SetReconocimientosOrigenAsignaturasDatosGestor(asignaturaReconocida.Asignatura.DatosGestor);

            return asignaturaReconocida;
        }

        protected internal virtual string SetNombreAsignaturaReconocida(int asignaturaId)
        {
            const string asignaturaReconocidaNombreSeminario = "09990011 - Seminario";
            const string asignaturaReconocidaNombreReconocimiento = "09990011 - Reconocimiento por Optatividad";
            const string asignaturaReconocidaNombreActividades = "09990011 - Actividades de Extensión Universitaria";

            return asignaturaId switch
            {
                AsignaturaReconocidaTipoReconocimiento => asignaturaReconocidaNombreReconocimiento,
                AsignaturaReconocidaTipoActividades => asignaturaReconocidaNombreActividades,
                _ => asignaturaId is AsignaturaReconocidaTipoSeminario1 or AsignaturaReconocidaTipoSeminario4
                    ? asignaturaReconocidaNombreSeminario
                    : string.Empty
            };
        }

        public virtual void SetReconocimientosOrigenAsignaturasDatosGestor(
            DatosGestorAlumnoPuedeTitularseErpAcademicoModel datosGestor)
        {
            const string tipoReconocimientoSeminario = "Seminario";
            const string tipoReconocimientoOptatividad = "Optatividad";
            const string tipoReconocimiento = "Reconocimiento";
            const string errorSeminarioIncorrecto = "Seminario indicado incorrectamente.";
            const string errorOptatividadIncorrecta = "Optatividad indicada incorrectamente.";
            const string errorReconocimientoIncorrecto = "Optatividad indicada incorrectamente.";

            if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoSeminario1 &&
                int.Parse(datosGestor.IdAsignaturaGestor) > 0)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoSeminario);
                datosGestor.ErroresReconocimientosOrigen.Add(errorSeminarioIncorrecto);
            }
            else if (int.Parse(datosGestor.IdAsignatura) > 0 && int.Parse(datosGestor.IdAsignaturaGestor) > 0 &&
                     datosGestor.ReconocimientosOrigen != null &&
                     datosGestor.ReconocimientosOrigen.Any(r => r.TipoReconocimiento == tipoReconocimientoSeminario))
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoSeminario);
                datosGestor.ErroresReconocimientosOrigen.Add(errorSeminarioIncorrecto);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoSeminario4 &&
                     int.Parse(datosGestor.IdAsignaturaGestor) > 0 ||
                     int.Parse(datosGestor.IdAsignatura) > 0 && int.Parse(datosGestor.IdAsignaturaGestor) ==
                     AsignaturaReconocidaTipoSeminario4)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoSeminario);
                datosGestor.ErroresReconocimientosOrigen.Add(errorSeminarioIncorrecto);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoSeminario4 &&
                     int.Parse(datosGestor.IdAsignaturaGestor) == AsignaturaReconocidaTipoSeminario4)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoSeminario);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoReconocimiento &&
                     int.Parse(datosGestor.IdAsignaturaGestor) != AsignaturaReconocidaTipoSeminario9)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoOptatividad);
                datosGestor.ErroresReconocimientosOrigen.Add(errorOptatividadIncorrecta);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoReconocimiento &&
                     int.Parse(datosGestor.IdAsignaturaGestor) == AsignaturaReconocidaTipoSeminario9)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimientoOptatividad);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoActividades &&
                     int.Parse(datosGestor.IdAsignaturaGestor) > 0 ||
                     int.Parse(datosGestor.IdAsignatura) > 0 && int.Parse(datosGestor.IdAsignaturaGestor) ==
                     AsignaturaReconocidaTipoActividades)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimiento);
                datosGestor.ErroresReconocimientosOrigen.Add(errorReconocimientoIncorrecto);
            }
            else if (int.Parse(datosGestor.IdAsignatura) == AsignaturaReconocidaTipoActividades &&
                     int.Parse(datosGestor.IdAsignaturaGestor) == AsignaturaReconocidaTipoActividades)
            {
                SetDatosTipoReconocimientoOrigen(datosGestor, tipoReconocimiento);
            }
        }

        public virtual void SetDatosTipoReconocimientoOrigen(
            DatosGestorAlumnoPuedeTitularseErpAcademicoModel datosGestor, string tipoReconocimiento)
        {
            if (datosGestor.ReconocimientosOrigen == null)
            {
                datosGestor.ReconocimientosOrigen = new List<ReconocimientosOrigenModel>
                {
                    new()
                    {
                        Ects = 0,
                        TipoReconocimiento = tipoReconocimiento
                    }
                };
                return;
            }

            if (tipoReconocimiento == TipoReconocimiento)
            {
                datosGestor.ReconocimientosOrigen.ForEach(r => r.TipoReconocimiento = r.NombreAsignaturaExterna);
                return;
            }

            datosGestor.ReconocimientosOrigen.ForEach(r => r.TipoReconocimiento = tipoReconocimiento);
        }

        protected internal virtual void SetElementosNoSuperados(int[] idsAsignaturasNoSuperadas,
            List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel> asignaturasBloqueNoSuperadas,
            AlumnoPuedeTitularseDto resultPlanSuperado)
        {
            resultPlanSuperado.ElementosNoSuperados.BloquesNoSuperados =
                SetBloquesAndSubBloquesNoSuperados(asignaturasBloqueNoSuperadas, resultPlanSuperado,
                    idsAsignaturasNoSuperadas);

            resultPlanSuperado.ElementosNoSuperados.AsignaturasPlanNoSuperadas.AddRange(resultPlanSuperado
                .EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas
                .Where(ap => idsAsignaturasNoSuperadas.Contains(ap.Id)).ToList());
        }

        protected internal virtual List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> SetBloquesAndSubBloquesNoSuperados(
            List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel> asignaturasBloqueNoSuperadas,
            AlumnoPuedeTitularseDto planSuperado, int[] idsAsignaturasNoSuperadas)
        {
            var result = new List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();
            var subBloquesNoSuperado = new List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();

            foreach (var idsAsignaturasNoSuperada in idsAsignaturasNoSuperadas)
            {
                ProcesarAsignaturaNoSuperadas(idsAsignaturasNoSuperada, planSuperado, result, subBloquesNoSuperado);
            }

            foreach (var asignatura in asignaturasBloqueNoSuperadas)
            {
                ProcesarAsignaturaBloqueNoSuperada(planSuperado, asignatura, result);
            }

            return result;
        }

        protected internal virtual void ProcesarAsignaturaNoSuperadas(int idAsignaturaNoSuperada,
            AlumnoPuedeTitularseDto planSuperado, List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> result,
            List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> subBloquesNoSuperado)
        {
            if (!planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                .SelectMany(sb => sb.SubBloquesSuperados).Any())
                return;

            var bloques = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                .Where(b => b.SubBloquesSuperados.Any(s =>
                    s.AsignaturasSubBloqueSuperadas.Any(asb => asb.IdAsignaturaPlan == idAsignaturaNoSuperada)));
            foreach (var bloque in bloques)
            {
                if (!result.Exists(b => b.Id == bloque.Id))
                {
                    SetSubBloquesNoSuperados(bloque, idAsignaturaNoSuperada, subBloquesNoSuperado, result);
                }
                else
                {
                    subBloquesNoSuperado.First().AsignaturasSubBloqueNoSuperadas.Add(
                        new AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel
                        {
                            IdAsignaturaPlan = idAsignaturaNoSuperada
                        });
                }
            }
        }

        protected internal virtual void SetSubBloquesNoSuperados(
            BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel bloque, int idAsignaturaPlanSubBloque,
            List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> subBloqueNoSuperado,
            List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> result)
        {
            var subBloques = bloque.SubBloquesSuperados.Where(b =>
                b.AsignaturasSubBloqueSuperadas.Any(abs =>
                    abs.IdAsignaturaPlan == idAsignaturaPlanSubBloque));
            foreach (var subBloque in subBloques)
            {
                subBloqueNoSuperado.Add(new SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = subBloque.Id,
                    Nombre = subBloque.Nombre,
                    Descripcion = subBloque.Descripcion,
                    CreditosMinimos = subBloque.CreditosMinimos,
                    CreditosObtenidos = 0,
                    AsignaturasSubBloqueNoSuperadas =
                        new List<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel>
                        {
                            new()
                            {
                                IdAsignaturaPlan = idAsignaturaPlanSubBloque
                            }
                        }
                });

                result.Add(new BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
                {
                    Id = bloque.Id,
                    Nombre = bloque.Nombre,
                    Descripcion = bloque.Descripcion,
                    CreditosMinimos = bloque.CreditosMinimos,
                    CreditosObtenidos = 0,
                    SubBloquesNoSuperados = subBloqueNoSuperado
                });
            }
        }

        protected internal virtual void ProcesarAsignaturaBloqueNoSuperada(AlumnoPuedeTitularseDto planSuperado,
            AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel asignaturaBloqueNoSuperada,
            List<BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> result)
        {
            var bloque = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.First(b =>
                b.AsignaturasBloqueSuperadas.Any(abs =>
                    abs.IdAsignaturaPlan == asignaturaBloqueNoSuperada.IdAsignaturaPlan));
            if (result.Exists(b => b.Id == bloque.Id))
            {
                return;
            }
            result.Add(new BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
            {
                Id = bloque.Id,
                Nombre = bloque.Nombre,
                Descripcion = bloque.Descripcion,
                CreditosMinimos = bloque.CreditosMinimos,
                CreditosObtenidos = 0,
                AsignaturasBloqueNoSuperadas =
                    new List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>
                    {
                        new()
                        {
                            IdAsignaturaPlan = asignaturaBloqueNoSuperada.IdAsignaturaPlan
                        }
                    }
            });
        }

        protected internal virtual void RemoverAsignaturasNoSuperadas(int[] idsAsignaturasNoSuperadas,
            AlumnoPuedeTitularseDto planSuperado)
        {
            foreach (var id in idsAsignaturasNoSuperadas)
            {
                var asignaturaPlan =
                    planSuperado.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Find(a => a.Id == id);
                planSuperado.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Remove(asignaturaPlan);
            }
        }

        protected internal virtual void RemoverBloquesAsignaturasNoSuperadas(
            List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel> asignaturasBloqueNoSuperadas,
            AlumnoPuedeTitularseDto planSuperado)
        {
            foreach (var asignatura in asignaturasBloqueNoSuperadas)
            {
                var asignaturaPlanBloque = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                    .SelectMany(a => a.AsignaturasBloqueSuperadas)
                    .ToList().Find(a => a.IdAsignaturaPlan == asignatura.IdAsignaturaPlan);

                if (asignaturaPlanBloque == null)
                {
                    continue;
                }

                planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                    .First(b => b.AsignaturasBloqueSuperadas.Any(abs =>
                        abs.IdAsignaturaPlan == asignaturaPlanBloque.IdAsignaturaPlan)).AsignaturasBloqueSuperadas
                    .Remove(asignaturaPlanBloque);
            }
        }

        public virtual void RemoverSubBloquesAsignaturasNoSuperadas(int[] idsAsignaturasNoSuperadas,
            AlumnoPuedeTitularseDto planSuperado)
        {
            foreach (var asignaturaSubBloque in idsAsignaturasNoSuperadas)
            {
                var asignaturaPlanSubBloque = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                    .SelectMany(s => s.SubBloquesSuperados)
                    .SelectMany(a => a.AsignaturasSubBloqueSuperadas)
                    .ToList().Find(a => a.IdAsignaturaPlan == asignaturaSubBloque);

                if (asignaturaPlanSubBloque != null)
                {
                    planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                        .SelectMany(sb => sb.SubBloquesSuperados)
                        .First(b => b.AsignaturasSubBloqueSuperadas.Any(abs =>
                            abs.IdAsignaturaPlan == asignaturaPlanSubBloque.IdAsignaturaPlan))
                        .AsignaturasSubBloqueSuperadas
                        .Remove(asignaturaPlanSubBloque);
                }
            }
        }

        protected internal virtual void SetAsignaturasConIntegracionErpAcademico(AsignaturaPlanAlumnoPuedeTitularseErpAcademicoModel asignaturaPlan,
            List<AsignaturaPlanTitulacionErpAcademico> datosAcademico, List<AsignaturaErpAcademicoExpedientesIntegrationModel> datosGestor)
        {
            var asignaturaAcademico =
                datosAcademico.FirstOrDefault(a => a.IdAsignaturaPlan == asignaturaPlan.Id.ToString());
            var asignaturaGestor = datosGestor.FirstOrDefault(g => g.IdAsignatura == asignaturaPlan.Id);

            if (asignaturaAcademico == null || asignaturaGestor == null)
                return;

            asignaturaPlan.Id = int.Parse(asignaturaAcademico.IdAsignaturaPlan);
            asignaturaPlan.Asignatura.Nombre = asignaturaAcademico.Nombre;
            asignaturaPlan.Asignatura.Codigo = asignaturaAcademico.Codigo;
            asignaturaPlan.Asignatura.Creditos = asignaturaAcademico.Creditos;
            asignaturaPlan.Asignatura.Curso = asignaturaAcademico.Curso != null ? asignaturaAcademico.Curso.ToString() : null;
            asignaturaPlan.Asignatura.CodigoOficialExterno = asignaturaAcademico.CodigoOficialExterno;
            asignaturaPlan.Asignatura.IdentificadorOficialExterno = asignaturaAcademico.IdentificadorOficialExterno;
            asignaturaPlan.Asignatura.Periodicidad = asignaturaAcademico.Periodicidad;
            asignaturaPlan.Asignatura.PeriodicidadCodigoOficialExterno = asignaturaAcademico.PeriodicidadCodigoOficialExterno;
            asignaturaPlan.Asignatura.PeriodoLectivo = asignaturaAcademico.PeriodoLectivo;
            asignaturaPlan.Asignatura.Idioma = asignaturaAcademico.Idioma;
            asignaturaPlan.Asignatura.TipoAsignatura = new TipoAsignaturaErpAcademicoModel
            {
                Id = asignaturaAcademico.TipoAsignatura.Id,
                Nombre = asignaturaAcademico.TipoAsignatura.Nombre,
                Orden = asignaturaAcademico.TipoAsignatura.Orden,
                Simbolo = asignaturaAcademico.TipoAsignatura.Simbolo,
                TrabajoFinEstudio = asignaturaAcademico.TipoAsignatura.TrabajoFinEstudio
            };
            asignaturaPlan.Asignatura.DatosGestor = new DatosGestorAlumnoPuedeTitularseErpAcademicoModel
            {
                IdAsignatura = asignaturaGestor.IdAsignatura.ToString(),
                IdAsignaturaGestor = asignaturaGestor.IdAsignaturaGestor.ToString(),
                Ects = asignaturaGestor.Ects.ToString(CultureInfo.InvariantCulture),
                NotaNumerica = asignaturaGestor.NotaNumerica.ToString(CultureInfo.InvariantCulture),
                NotaAlfanumerica = asignaturaGestor.NotaAlfanumerica,
                Superado = asignaturaGestor.Superado,
                AnyoAcademico = asignaturaGestor.AnyoAcademico,
                Convocatoria = asignaturaGestor.Convocatoria,
                Observaciones = asignaturaGestor.Observaciones,
                ReconocimientosOrigen = asignaturaGestor.ReconocimientosOrigen
            };
        }

        protected internal virtual AsignaturaErpAcademicoExpedientesIntegrationModel GetAsignaturaGestor(
            int asignaturaPlanId, List<AsignaturaErpAcademicoExpedientesIntegrationModel> datosGestor)
        {
            var listaAsignaturaGestor = datosGestor.Where(g => g.IdAsignatura == asignaturaPlanId).ToList();
            if (!listaAsignaturaGestor.Any()) return null;

            if (listaAsignaturaGestor.Count > 1)
            {
                return listaAsignaturaGestor.Where(ag => ag.Superado).MaxBy(ag => ag.NotaNumerica)
                       ?? listaAsignaturaGestor.Where(ag => !ag.Superado).MaxBy(ag => ag.NotaNumerica);
            }

            return listaAsignaturaGestor.First();
        }

        protected internal virtual async Task SetExpedicion(AlumnoPuedeTitularseDto planSuperado,
            ExpedienteExpedientesIntegrationModel expedicion, int idRefPlan)
        {
            planSuperado.Expedicion = new ExpedicionDto
            {
                ViaAcceso = expedicion.ViaAcceso,
                FechaFinEstudio = expedicion.FechaFinEstudio,
                FechaExpedicion = expedicion.FechaExpedicion,
                TituloTfmTfg = expedicion.TituloTfmTfg,
                FechaTfmTfg = expedicion.FechaTfmTfg,
                NotaMedia = expedicion.NotaMedia
            };

            if (expedicion.ViasAcceso != null)
            {
                planSuperado.Expedicion.ViasAcceso = new ViasAccesoExpedicionDto
                {
                    Generica = expedicion.ViasAcceso.Generica,
                    Especifica = expedicion.ViasAcceso.Especifica,
                    GenericaIngles = expedicion.ViasAcceso.GenericaIngles,
                    EspecificaIngles = expedicion.ViasAcceso.EspecificaIngles
                };
            }

            if (expedicion.IdiomaAcreditacion != null)
            {
                planSuperado.Expedicion.IdiomaAcreditacion = new IdiomaAcreditacionExpedicionDto
                {
                    Idioma = expedicion.IdiomaAcreditacion.Idioma,
                    Acreditacion = expedicion.IdiomaAcreditacion.Acreditacion,
                    FechaAcreditacion = expedicion.IdiomaAcreditacion.FechaAcreditacion
                };
            }

            if (expedicion.ItinerariosFinalizados != null)
            {
                foreach (var itinerarioFinalizadoIntegrado in expedicion.ItinerariosFinalizados)
                {
                    var asignaturasAsociadas =
                        await _erpAcademicoServiceClient.GetAsignaturasEspecializacionPlan(
                            int.Parse(itinerarioFinalizadoIntegrado.IdEspecializacionErp), idRefPlan);
                    var itinerarioFinalizado = new ItinerariosFinalizadosExpedicionDto
                    {
                        TipoItinerario = new TipoItinerarioExpedicionDto
                        {
                            Id = itinerarioFinalizadoIntegrado.TipoItinerario.Id.ToString(),
                            Nombre = itinerarioFinalizadoIntegrado.TipoItinerario.Nombre
                        },
                        Nombre = itinerarioFinalizadoIntegrado.Nombre,
                        FechaFin = itinerarioFinalizadoIntegrado.FechaFin,
                        IdEspecializacionErp = itinerarioFinalizadoIntegrado.IdEspecializacionErp,
                        AsignaturasAsociadas = asignaturasAsociadas
                    };
                    planSuperado.Expedicion.ItinerariosFinalizados.Add(itinerarioFinalizado);
                }
            }

            if (expedicion.Practica != null)
            {
                planSuperado.Expedicion.Practica = new PracticaExpedicionDto
                {
                    CentroPractica = expedicion.Practica.CentroPractica,
                    FechaInicio = expedicion.Practica.FechaInicio,
                    FechaFin = expedicion.Practica.FechaFin
                };
            }
        }

        protected internal virtual void SetCreditosObtenidosAsignaturasSuperadas(AlumnoPuedeTitularseDto planSuperado)
        {
            var idsAsignaturasSuperadas = planSuperado.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Select(a => a.Id).ToList();

            if (planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Any())
            {
                planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.ForEach(b => b.CreditosObtenidos = 0);
            }

            if (planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Any(sb => sb.SubBloquesSuperados.Any()))
            {
                planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.ForEach(b =>
                    b.SubBloquesSuperados.ForEach(sb => sb.CreditosObtenidos = 0));
            }

            var bloques = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.ToList();

            foreach (var bloque in bloques)
            {
                foreach (var asignatura in idsAsignaturasSuperadas)
                {
                    if (bloque.AsignaturasBloqueSuperadas.Any() &&
                        bloque.AsignaturasBloqueSuperadas.Any(a => a.IdAsignaturaPlan == asignatura))
                    {
                        planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                            .Where(b => b.Id == bloque.Id)
                            .First(a => a.AsignaturasBloqueSuperadas.Any(ab => ab.IdAsignaturaPlan == asignatura))
                            .CreditosObtenidos += GetCreditosObtenidosByAsignatura(planSuperado, asignatura);
                    }
                    else if (bloque.SubBloquesSuperados.Any() && bloque.SubBloquesSuperados.Any(sb =>
                        sb.AsignaturasSubBloqueSuperadas.Any(a => a.IdAsignaturaPlan == asignatura)))
                    {
                        foreach (var _ in planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                            .Where(b => b.Id == bloque.Id)
                            .Select(sb => sb.SubBloquesSuperados
                                    .First(asb => asb.AsignaturasSubBloqueSuperadas
                                        .Any(a => a.IdAsignaturaPlan == asignatura)).CreditosObtenidos +=
                                GetCreditosObtenidosByAsignatura(planSuperado, asignatura)))
                        {
                        }

                        planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                                .First(b => b.Id == bloque.Id).CreditosObtenidos +=
                            GetCreditosObtenidosByAsignatura(planSuperado, asignatura);
                    }
                }
            }
        }

        protected internal virtual double GetCreditosObtenidosByAsignatura(AlumnoPuedeTitularseDto planSuperado,
            int idAsignatura)
        {
            return planSuperado.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas
                .First(a => a.Id == idAsignatura).Asignatura.Creditos;
        }

        protected internal virtual void SetArcosSuperados(AlumnoPuedeTitularseDto planSuperado)
        {
            planSuperado.EsPlanSuperado.ElementosSuperados.Arcos.ForEach(arcos => { arcos.Superado = true; });

            var bloquesSuperados = planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.Select(b => b.Id)
                .ToList();

            foreach (var bloqueSuperado in bloquesSuperados)
            {
                if (planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados.First(b => b.Id == bloqueSuperado)
                    .CreditosObtenidos < planSuperado.EsPlanSuperado.ElementosSuperados.BloquesSuperados
                    .First(b => b.Id == bloqueSuperado).CreditosMinimos)
                {
                    planSuperado.EsPlanSuperado.ElementosSuperados.Arcos
                        .First(b => b.BloquesSuperados.Contains(bloqueSuperado)).Superado = false;
                }
            }
        }

        protected internal virtual void SetBloqueos(AlumnoPuedeTitularseDto planSuperado, List<BloqueosIntegrationModel> bloqueos)
        {
            foreach (var bloqueo in bloqueos)
            {
                planSuperado.Bloqueos = new BloqueoExpedienteAlumnoDto
                {
                    Bloqueado = true,
                    Nombre = bloqueo.Nombre,
                    Observacion = bloqueo.Observacion,
                    AccionesBloquedas = SetAccionesBloqueadas(bloqueo.AccionesBloquedas)
                };
            }
        }

        protected internal virtual List<AccionBloqueoDto> SetAccionesBloqueadas(List<AccionesBloqueadasIntegrationModel> accionesBloqueadas)
        {
            var result = new List<AccionBloqueoDto>();
            if (accionesBloqueadas != null)
            {
                result = accionesBloqueadas.Select(accion => new AccionBloqueoDto
                {
                    CodigoAccion = accion.CodigoAccion
                }).ToList();
            }

            return result;
        }

        protected internal virtual async Task<ValidateAlumnoMatriculacionErpAcademicoModel> GetCausasFallosComprobacionMatriculasDocumentacionErp(string idIntegracionAlumno, string idRefPlan)
        {
            var parameters = new ValidateAlumnoMatriculacionParameters
            {
                IdIntegracionAlumno = idIntegracionAlumno,
                IdRefPlan = idRefPlan
            };
            var resultCheckMatriculas = await
                _erpAcademicoServiceClient.ValidateAlumnoMatriculacion(parameters);

            return resultCheckMatriculas ?? throw new BadRequestException(_localizer["[ERP Académico]: No se ha podido obtener los datos de la Matriculación del Alumno."]);
        }

        public virtual void FiltrarAsignaturasExpedienteAlumno(AlumnoPuedeTitularseDto expedienteAlumno,
            GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosQuery expedienteAlumnoParameters)
        {
            const string asignaturaNoPresentada = "-1";
            const string asignaturaMatriculada = "-12";

            var asignaturasSuperadas = expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas
                .Where(a => a.Asignatura.DatosGestor.Superado).ToList();
            var asignaturasSuspensas = expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.Where(a =>
                !a.Asignatura.DatosGestor.Superado &&
                !a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaNoPresentada) &&
                !a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaMatriculada)).ToList();

            var asignaturasMatriculadasPlanSuperado = expedienteAlumno.EsPlanSuperado.ElementosSuperados
                .AsignaturasPlanSuperadas
                .Where(a => a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaMatriculada)).ToList();
            var asignaturasMatriculadasPlanNoSuperado = expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas
                .Where(a => a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaMatriculada)).ToList();

            var asignaturasNoPresentadasPlanSuperado = expedienteAlumno.EsPlanSuperado.ElementosSuperados
                .AsignaturasPlanSuperadas
                .Where(a => a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaNoPresentada)).ToList();
            var asignaturasNoPresentadasPlanNoSuperado = expedienteAlumno.ElementosNoSuperados
                .AsignaturasPlanNoSuperadas
                .Where(a => a.Asignatura.DatosGestor.NotaNumerica.Equals(asignaturaNoPresentada)).ToList();

            if (expedienteAlumnoParameters.ConAsignaturas.HasValue && expedienteAlumnoParameters.ConAsignaturas.Value)
            {
                if (expedienteAlumnoParameters.TodasAsignaturas.HasValue && !expedienteAlumnoParameters.TodasAsignaturas.Value)
                {
                    expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Clear();
                    expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.Clear();
                }

                if (expedienteAlumnoParameters.AsignaturasSuperadas.HasValue && expedienteAlumnoParameters.AsignaturasSuperadas.Value)
                {
                    expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.AddRange(
                        asignaturasSuperadas);
                }

                if (expedienteAlumnoParameters.AsignaturasSuspensas.HasValue && expedienteAlumnoParameters.AsignaturasSuspensas.Value)
                {
                    expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.AddRange(asignaturasSuspensas);
                }

                if (expedienteAlumnoParameters.AsignaturasMatriculadas.HasValue && expedienteAlumnoParameters.AsignaturasMatriculadas.Value)
                {
                    expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.AddRange(
                        asignaturasMatriculadasPlanSuperado);
                    expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.AddRange(
                        asignaturasMatriculadasPlanNoSuperado);
                }

                if (expedienteAlumnoParameters.AsignaturasNoPresentadas.HasValue && expedienteAlumnoParameters.AsignaturasNoPresentadas.Value)
                {
                    expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.AddRange(
                        asignaturasNoPresentadasPlanSuperado);
                    expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.AddRange(
                        asignaturasNoPresentadasPlanNoSuperado);
                }
            }
            else
            {
                expedienteAlumno.EsPlanSuperado.ElementosSuperados.AsignaturasPlanSuperadas.Clear();
                expedienteAlumno.ElementosNoSuperados.AsignaturasPlanNoSuperadas.Clear();
            }
        }
    }
}
