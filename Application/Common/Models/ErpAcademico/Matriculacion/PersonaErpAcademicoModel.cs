using System;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class PersonaErpAcademicoModel
    {
        public string Foto { get; set; }
        public string Sexo { get; set; }
        public string Celular { get; set; }
        public string IdRefPaisNacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DocumentoIdentificacionModel[] DocumentosIdentificacion { get; set; }
    }
}
