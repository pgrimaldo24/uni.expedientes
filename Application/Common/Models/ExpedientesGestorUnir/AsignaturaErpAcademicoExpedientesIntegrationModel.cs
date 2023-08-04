using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir
{
    public class AsignaturaErpAcademicoExpedientesIntegrationModel
    {
        public const int NotaNumericaMatriculada = -12;
        public const int NotaNumericaNoPresentada = -1;
        public const string NotaAlfanumericaMatriculaHonor = "Matrícula de Honor";

        public int IdAsignatura { get; set; }
        public int IdAsignaturaGestor { get; set; }
        public double Ects { get; set; }
        public double NotaNumerica { get; set; }
        public string NotaAlfanumerica { get; set; }
        public bool Superado { get; set; }
        public string AnyoAcademico { get; set; }
        public string Convocatoria { get; set; }
        public string Observaciones { get; set; }
        public List<ReconocimientosOrigenModel> ReconocimientosOrigen { get; set; }
        public virtual bool GetAsignaturasReconocidas()
        {
            return IdAsignatura < 0;
        }
    }
}
