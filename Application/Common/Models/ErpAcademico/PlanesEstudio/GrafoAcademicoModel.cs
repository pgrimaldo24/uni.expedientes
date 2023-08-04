using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class GrafoDataAcademicoModel
    {
        public GrafoAcademicoModel Data { get; set; }
    }

    public class GrafoAcademicoModel
    {
        public bool TienePosicionGrafica { get; set; }
        public List<NodoAcademicoModel> Nodos { get; set; }
    }

    public class NodoAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public TipoNodoAcademicoModel Tipo { get; set; }
        public List<NodoAcademicoModel> Hijos { get; set; }
        public List<ArcoAcademicoModel> ArcosSalientes { get; set; }
    }

    public class ArcoAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<BloqueAcademicoModel> Bloques { get; set; }
        public NodoDestinoAcademicoModel NodoDestino { get; set; }
    }

    public class BloqueAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal MinCreditos { get; set; }
        public List<AsignaturaErpAcademicoModel> Asignaturas { get; set; }
        public List<BloqueAcademicoModel> SubBloques { get; set; }
    }

    public class NodoDestinoAcademicoModel
    {
        public int Id { get; set; }
    }
}
