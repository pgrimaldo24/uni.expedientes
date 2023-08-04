﻿namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class TipoAsignaturaErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public string Simbolo { get; set; }
        public bool TrabajoFinEstudio { get; set; }
        public string Codigo => Simbolo;
    }
}
