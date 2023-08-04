using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseAsignaturaErpAcademicoExpedientesIntegrationModel
    {
        public int IdAsignatura { get; set; }
        public int IdAsignaturaGestor { get; set; }
        public double Ects { get; set; }
        public double NotaNumerica { get; set; }
        public string NotaAlfanumerica { get; set; }
        public bool Superado { get; set; }
        public string AnyoAcademico { get; set; }
        public string Convocatoria { get; set; }
        public string Observaciones { get; set; }
        public List<ResponseReconocimientosOrigenModel> ReconocimientosOrigen { get; set; }
    }
}
