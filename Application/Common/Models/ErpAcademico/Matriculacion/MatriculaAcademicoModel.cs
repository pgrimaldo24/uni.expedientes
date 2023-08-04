using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class MatriculaAcademicoModel
    {
        public MatriculaAcademicoModel()
        {
            AsignaturaMatriculadas = new List<AsignaturaMatriculadaModel>();
        }
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public string DisplayName { get; set; }
        public string IdRefExpedienteAlumno { get; set; }
        public decimal TotalCreditosAsignaturasMatriculadasActivas { get; set; }
        public DateTime FechaCambioEstado { get; set; }
        public TipoMatriculaAcademicoModel Tipo { get; set; }
        public PlanOfertadoDtoAcademicoModel PlanOfertado { get; set; }
        public RegionEstudioAcademicoModel RegionEstudio { get; set; }
        public EstadoMatriculaAcademicoModel Estado { get; set; }
        public List<AsignaturaMatriculadaModel> AsignaturaMatriculadas { get; set; }
    }

    public class TipoMatriculaAcademicoModel
    {
        public string DisplayName { get; set; }
        public bool PermiteCrearPrimeraMatricula { get; set; }
    }

    public class RegionEstudioAcademicoModel
    {
        public string DisplayName { get; set; }
    }

    public class EstadoMatriculaAcademicoModel
    {
        public const int PREMATRICULA = 1;
        public const int RECIBIDA = 2;
        public const int ALTA = 3;
        public const int BAJA = 4;
        public const int DESESTIMADA = 5;
        public string DisplayName { get; set; }
        public bool EsDesestimada { get; set; }
        public bool EsAlta { get; set; }
        public bool EsBaja { get; set; }
        public bool EsRecibida { get; set; }
        public bool EsPrematricula { get; set; }
        public bool EsActiva => EsPrematricula || EsRecibida || EsAlta;
    }

    public class PlanOfertadoDtoAcademicoModel
    {
        public PeriodoAcademicoAcademicoModel PeriodoAcademico { get; set; }
        public PlanAcademicoModel Plan { get; set; }
    }

    public class PeriodoAcademicoAcademicoModel
    {
        public string DisplayName { get; set; }
        public string FechaInicio { get; set; }
        public AnyoAcademicoAcademicoModel AnyoAcademico { get; set; }
    }

    public class AnyoAcademicoAcademicoModel
    {
        public string DisplayName { get; set; }
    }
}
