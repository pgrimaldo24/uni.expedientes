using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RolAnotacion : Entity<int>
    {
        public int AnotacionId { get; set; }
        public Anotacion Anotacion { get; set; }
        [StringLength(50)] public string Rol { get; set; }
    }
}
