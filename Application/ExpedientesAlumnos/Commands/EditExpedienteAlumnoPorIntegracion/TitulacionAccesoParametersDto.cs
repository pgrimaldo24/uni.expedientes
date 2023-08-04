using System;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion
{
    public class TitulacionAccesoParametersDto
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
    }

    public class ExpedienteEspecializacionDto
    {
        public string IdRefEspecializacion { get; set; }
    }
}
