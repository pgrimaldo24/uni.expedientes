using System;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponsePersonaAcademico
    {
        public string Foto { get; set; }
        public string Sexo { get; set; }
        public string Celular { get; set; }
        public string IdRefPaisNacionalidad { get; set; }
        public string FechaNacimiento { get; set; }
        public ResponseDocumentoIdentificacionAcademico[] DocumentosIdentificacion { get; set; }
    }
}
