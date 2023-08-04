using System;
using System.Collections.Generic;
using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.CreateExpedienteAlumno
{
    public class CreateExpedienteAlumnoCommand : IRequest<int>
    {
        public string IdRefIntegracionAlumno { get; set; }
        public string IdRefPlan { get; set; }
        public string IdRefVersionPlan { get; set; }
        public int? NroVersion { get; set; }
        public string IdRefNodo { get; set; }
        public bool PorIntegracion { get; set; }
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
        public TitulacionAccesoDto TitulacionAcceso { get; set; }
        public List<ExpedienteEspecializacionDto> Especializaciones { get; set; }
        public List<int> IdsPlanes { get; set; }
        public DateTime? FechaHoraMensaje { get; set; }
        public string IdIntegracionMatricula { get; set; }
        public int IdEstadoMatricula { get; set; }
    }
}
