using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById
{
    public class ExpedienteAlumnoItemDto : IMapFrom<ExpedienteAlumno>
    {
        public int Id { get; set; }
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefVersionPlan { get; set; }
        public string IdRefNodo { get; set; }
        public string AlumnoNombre { get; set; }
        public string AlumnoApellido1 { get; set; }
        public string AlumnoApellido2 { get; set; }
        public string IdRefTipoDocumentoIdentificacionPais { get; set; }
        public string AlumnoNroDocIdentificacion { get; set; }
        public string AlumnoEmail { get; set; }
        public string IdRefViaAccesoPlan { get; set; }

        public string DocAcreditativoViaAcceso { get; set; }
        public string IdRefIntegracionDocViaAcceso { get; set; }
        public string FechaSubidaDocViaAcceso { get; set; }
        public string IdRefTipoVinculacion { get; set; }
        public string NombrePlan { get; set; }
        public string IdRefUniversidad { get; set; }
        public string AcronimoUniversidad { get; set; }
        public string IdRefCentro { get; set; }
        public string IdRefAreaAcademica { get; set; }
        public string IdRefTipoEstudio { get; set; }
        public string IdRefEstudio { get; set; }
        public string NombreEstudio { get; set; }
        public string IdRefTitulo { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public DateTime? FechaTrabajoFinEstudio { get; set; }
        public string TituloTrabajoFinEstudio { get; set; }
        public DateTime? FechaExpedicion { get; set; }
        public double? NotaMedia { get; set; }
        public DateTime? FechaPago { get; set; }
        public string DisplayNameIdIntegracion => $"{IdRefIntegracionAlumno} - {AlumnoNombre} {AlumnoApellido1} {AlumnoApellido2}";
        public string DisplayNameDocumentoIdentificacionAlumno =>
            $"{IdRefIntegracionAlumno} - {IdRefTipoDocumentoIdentificacionPais}/{AlumnoNroDocIdentificacion}".TrimEnd();
        public string DisplayNameNombreAlumno => $"{AlumnoNombre} {AlumnoApellido1} {AlumnoApellido2}".TrimEnd();
        public List<ExpedienteEspecializacionDto> ExpedientesEspecializaciones { get; set; }
        public virtual TitulacionAccesoDto TitulacionAcceso { get; set; }
        public List<ConsolidacionRequisitoExpedienteDto> ConsolidacionesRequisitosExpedientes { get; set; }
        public AlumnoDto Alumno { get; set; }
        public EstadoExpedienteDto Estado { get; set; }
    }

    public class EstadoExpedienteDto: IMapFrom<EstadoExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Color { get; set; }
    }

    public class ExpedienteEspecializacionDto : IMapFrom<ExpedienteEspecializacion>
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class TitulacionAccesoDto : IMapFrom<TitulacionAcceso>
    {
        public string Titulo { get; set; }
        public string InstitucionDocente { get; set; }
        public int? NroSemestreRealizados { get; set; }
        public string TipoEstudio { get; set; }
        public string IdRefTerritorioInstitucionDocente { get; set; }
        public DateTime? FechaInicioTitulo { get; set; }
        public DateTime? FechafinTitulo { get; set; }
        public string CodigoColegiadoProfesional { get; set; }
        public string IdRefInstitucionDocente { get; set; }
        public string CodeCountryInstitucionDocente { get; set; }
    }

    public class AlumnoDto
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
    }
}
