namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseAsignaturaPlanSubBloqueErpAcademico
    {
        public int IdAsignaturaPlan { get; set; }
        public virtual ResponseTipoAsignaturaErpAcademico TipoAsignatura { get; set; }
    }
}
