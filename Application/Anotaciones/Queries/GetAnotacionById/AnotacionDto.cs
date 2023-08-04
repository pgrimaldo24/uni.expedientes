using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.Anotaciones.Queries.GetAnotacionById
{
    public class AnotacionDto : IMapFrom<Anotacion>
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string IdRefCuentaSeguridad { get; set; }
        public bool EsPublica { get; set; }
        public bool EsRestringida { get; set; }
        public List<RolesAnotacionesDto> RolesAnotaciones { get; set; }
        public string Resumen { get; set; }
        public string Mensaje { get; set; }
        public string NombreUsuario { get; set; }
        public ExpedienteAlumnoAnotacionDto ExpedienteAlumno { get; set; }
    }

    public class RolesAnotacionesDto : IMapFrom<RolAnotacion>
    {
        public string Rol { get; set; }
        public string RolName => Rol == AppConfiguration.KeyAdminRole 
            ? AppConfiguration.KeyAdminRoleName : AppConfiguration.KeyGestorRoleName;
    }

    public class ExpedienteAlumnoAnotacionDto : IMapFrom<ExpedienteAlumno>
    {
        public int Id { get; set; }
    }
}
