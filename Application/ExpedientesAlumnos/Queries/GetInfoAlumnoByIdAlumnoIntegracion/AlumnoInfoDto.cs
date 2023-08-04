using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion
{
    public class AlumnoInfoDto
    {
        public int IdAlumno { get; set; }
        public string IdIntegracionAlumno { get; set; }
        public string DisplayName { get; set; }
        public string Foto { get; set; }
        public string Sexo { get; set; }
        public string Celular { get; set; }
        public string Nacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Email { get; set; }
        public string TipoDocumentoIdentificacionPais { get; set; }
        public string NroDocIdentificacion { get; set; }
        public string AcronimoUniversidad { get; set; }
        public string IdUniversidadIntegracion { get; set; }
        public DocumentoIdentificacionModel[] DocumentosIdentificacion { get; set; }
        public ICollection<DocumentoAlumnoAcademicoModel> DocumentosAlumno { get; set; }
        public ICollection<ExpedienteDto> Expedientes { get; set; }
        public ICollection<MatriculaDto> Matriculas { get; set; }
    }

    public class ExpedienteDto : IMapFrom<ExpedienteAlumno>
    {
        public int Id { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string NombrePlan { get; set; }
        public string NombreEstudio { get; set; }
        public string AcronimoUniversidad { get; set; }
        public List<ConsolidacionRequisitoExpedienteDto> ConsolidacionesRequisitosExpedientes { get; set; }
    }

    public class ConsolidacionRequisitoExpedienteDto : IMapFrom<ConsolidacionRequisitoExpediente>
    {
        public int Id { get; set; }
        public bool EsDocumentacionFisica { get; set; }
        public bool DocumentacionRecibida { get; set; }
        public bool EnviadaPorAlumno { get; set; }
        public string IdRefIdioma { get; set; }
        public string SiglasIdioma { get; set; }
        public string NombreIdioma { get; set; }
        public string NivelIdioma { get; set; }
        public string Texto { get; set; }
        public DateTime? Fecha { get; set; }
        public bool IsEstadoNoProcesada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.NoProcesado;
        public bool IsEstadoValidada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Validado;
        public bool IsEstadoPendiente => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Pendiente;
        public bool IsEstadoRechazada => EstadoRequisitoExpediente?.Id == Domain.Entities.EstadoRequisitoExpediente.Rechazado;
        public EstadoRequisitoExpedienteDto EstadoRequisitoExpediente { get; set; }
        public TipoRequisitoExpedienteDto TipoRequisitoExpediente { get; set; }
        public virtual RequisitoExpedienteDto RequisitoExpediente { get; set; }
    }

    public class EstadoRequisitoExpedienteDto : IMapFrom<EstadoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class TipoRequisitoExpedienteDto : IMapFrom<TipoRequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class RequisitoExpedienteDto : IMapFrom<RequisitoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public string Descripcion { get; set; }
    }

    public class MatriculaDto
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public string DisplayName { get; set; }
        public string IdRefExpedienteAlumno { get; set; }
        public decimal TotalCreditosAsignaturasMatriculadasActivas { get; set; }
        public TipoMatriculaDto Tipo { get; set; }
        public PlanOfertadoDtoDto PlanOfertado { get; set; }
        public RegionEstudioDto RegionEstudio { get; set; }
        public EstadoMatriculaDto Estado { get; set; }
    }

    public class TipoMatriculaDto
    {
        public string DisplayName { get; set; }
    }

    public class RegionEstudioDto
    {
        public string DisplayName { get; set; }
    }

    public class EstadoMatriculaDto
    {
        public string DisplayName { get; set; }
    }

    public class PlanOfertadoDtoDto
    {
        public PeriodoAcademicoDto PeriodoAcademico { get; set; }
        public PlanDto Plan { get; set; }
    }

    public class PeriodoAcademicoDto
    {
        public string DisplayName { get; set; }
        public string FechaInicio { get; set; }
        public AnyoAcademicoDto AnyoAcademico { get; set; }
    }

    public class AnyoAcademicoDto
    {
        public string DisplayName { get; set; }
    }

    public class PlanDto
    {
        public string DisplayName { get; set; }
    }
}
