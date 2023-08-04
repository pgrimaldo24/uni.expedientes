using System;
using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTitulacionAccesoExpedienteAlumno
{
    public class EditTitulacionAccesoExpedienteAlumnoCommand : IRequest
    {
        public int IdExpedienteAlumno { get; set; }
        public string Titulo { get; set; }
        public string InstitucionDocente { get; set; }
        public int? NroSemestreRealizados { get; set; }
        public string TipoEstudio { get; set; }
        public string IdRefTerritorioInstitucionDocente { get; set; }
        public DateTime? FechaInicioTitulo { get; set; }
        public DateTime? FechafinTitulo { get; set; }
        public string CodigoColegiadoProfesional { get; set; }
        public string IdRefInstitucionDocente { get; set; }

        public EditTitulacionAccesoExpedienteAlumnoCommand(int idExpedienteAlumno, string titulo,
            string institucionDocente, int? nroSemestreRealizados, string tipoEstudio, 
            string idRefTerritorioInstitucionDocente, DateTime? fechaInicioTitulo, DateTime? fechaFinTitulo, 
            string codigoColegiadoProfesional, string idRefInstitucionDocente)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
            Titulo = titulo;
            InstitucionDocente = institucionDocente;
            NroSemestreRealizados = nroSemestreRealizados;
            TipoEstudio = tipoEstudio;
            IdRefTerritorioInstitucionDocente = idRefTerritorioInstitucionDocente;
            FechaInicioTitulo = fechaInicioTitulo;
            FechafinTitulo = fechaFinTitulo;
            CodigoColegiadoProfesional = codigoColegiadoProfesional;
            IdRefInstitucionDocente = idRefInstitucionDocente;
        }
    }
}
