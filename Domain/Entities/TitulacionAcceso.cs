using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TitulacionAcceso : Entity<int>
    {
        [StringLength(250)]
        public string Titulo { get; set; }

        [StringLength(250)]
        public string InstitucionDocente { get; set; }
        
        public int? NroSemestreRealizados { get; set; }

        [StringLength(150)]
        public string TipoEstudio { get; set; }

        [StringLength(50)]
        public string IdRefTerritorioInstitucionDocente { get; set; }
        
        public DateTime? FechaInicioTitulo { get; set; }
        
        public DateTime? FechafinTitulo { get; set; }

        [StringLength(50)]
        public string CodigoColegiadoProfesional { get; set; }

        public virtual ICollection<ExpedienteAlumno> ExpedientesAlumnos { get; set; }

        [StringLength(50)]
        public string IdRefInstitucionDocente { get; set; }
    }
}
