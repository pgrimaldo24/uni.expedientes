namespace Unir.Expedientes.Application.Common.Parameters
{
    public class ExpedienteEditParameters
    {
        public ViaAccesoParameters ViaAccesoIntegracion { get; set; }
        public TitulacionAccesoParameters TitulacionAccesoIntegracion { get; set; }
    }

    public class ViaAccesoParameters
    {
        public int Id { get; set; }
    }

    public class TitulacionAccesoParameters
    {
        public string Titulo { get; set; }
        public string InstitucionDocente { get; set; }
        public int? NroSemestreRealizados { get; set; }
    }
}
