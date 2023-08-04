using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ExpedienteEspecializacion : Entity<int>
    {
        public int ExpedienteAlumnoId { get; set; }
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        
        [StringLength(50)]
        public string IdRefEspecializacion { get; set; }

        public virtual ICollection<HitoConseguido> HitosConseguidos { get; set; }
    }
}
