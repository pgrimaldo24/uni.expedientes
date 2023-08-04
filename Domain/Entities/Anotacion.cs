using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class Anotacion : Entity<int>
    {
        public DateTime Fecha { get; set; }
        [StringLength(50)] public string IdRefCuentaSeguridad { get; set; }
        public bool EsPublica { get; set; }
        public bool EsRestringida { get; set; }
        [StringLength(200)] public string Resumen { get; set; }
        public string Mensaje { get; set; }
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public DateTime FechaModificacion { get; set; }
        [StringLength(50)] public string IdRefCuentaSeguridadModificacion { get; set; }
        public ICollection<RolAnotacion> RolesAnotaciones { get; set; }

        public virtual void DeleteRolesNoIncluidos(string[] roles)
        {
            var rolesAEliminar = roles == null ? RolesAnotaciones
                : RolesAnotaciones.Where(ra => !roles.Contains(ra.Rol));
            foreach (var rolAnotacion in rolesAEliminar)
            {
                RolesAnotaciones.Remove(rolAnotacion);
            }
        }
    }
}
