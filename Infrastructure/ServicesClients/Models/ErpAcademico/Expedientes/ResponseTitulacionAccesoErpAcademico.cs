namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Expedientes
{
    public class ResponseTitulacionAccesoErpAcademico
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string InstitucionDocente { get; set; }
        public int? NroSemestreRealizados { get; set; }
    }
}
