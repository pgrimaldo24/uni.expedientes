using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseGrafoDataErpAcademico
    {
        public ResponseGrafoErpAcademico Data { get; set; }
    }

    public class ResponseGrafoErpAcademico
    {
        public bool TienePosicionGrafica { get; set; }
        public List<ResponseNodoAcademico> Nodos { get; set; }
    }

    public class ResponseNodoAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CountHitos { get; set; }
        public ResponseTipoNodoErpAcademico Tipo { get; set; }
        public List<ResponseNodoAcademico> Hijos { get; set; }
        public List<ResponseArcoAcademico> ArcosSalientes { get; set; }
    }

    public class ResponseArcoAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<ResponseBloqueAcademico> Bloques { get; set; }
        public ResponseNodoDestinoAcademico NodoDestino { get; set; }
    }

    public class ResponseBloqueAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal MinCreditos { get; set; }
        public List<ResponseAsignaturaErpAcademico> Asignaturas { get; set; }
        public List<ResponseBloqueAcademico> SubBloques { get; set; }
    }

    public class ResponseNodoDestinoAcademico
    {
        public int Id { get; set; }
    }
}
