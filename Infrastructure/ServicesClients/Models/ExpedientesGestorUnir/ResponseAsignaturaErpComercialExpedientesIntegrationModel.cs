using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedientesGestorUnir
{
    public class ResponseAsignaturaErpComercialExpedientesIntegrationModel
    {
        public ResponseAsignaturaErpComercialExpedientesIntegrationModel()
        {
            ReconocimientosOrigen = new List<ResponseReconocimientosOrigenModel>();
        }

        internal const int EstadoAsignaturaExpedienteMatriculada = 10,
            EstadoAsignaturaExpedienteNoPresentado = 20,
            EstadoAsignaturaExpedienteSuspendida = 30,
            EstadoAsignaturaExpedienteSuperada = 40,
            EstadoAsignaturaExpedienteMatriculaDeHonor = 50,
            EstadoAsignaturaExpedienteEnProcesoReconocimiento = 60,
            EstadoAsignaturaExpedienteReconocida = 70;

        /// <summary>
        /// Se define el nombre del estado porque la aplicacion de expediente no lo persiste, 
        /// solo calcula el estado en base al rango de estados.
        /// </summary>
        public const string NombreEstadoMatriculada = "MATRICULADA",
            NombreEstadoNoPresentado = "NO PRESENTADO",
            NombreEstadoSuspendida = "SUSPENDIDA",
            NombreEstadoSuperada = "SUPERADA",
            NombreEstadoMatriculaDeHonor = "MATRICULA DE HONOR",
            NombreEstadoEnProcesoReconocimiento = "EN PROCESO DE RECONOCIMIENTO",
            NombreEstadoReconocida = "RECONOCIDA";

        /// <summary>
        /// Identifica si la Asignatura no esta Mapeado en la app del gestor de mapeo
        /// </summary>
        internal const int AsignaturaNoMapeada = -1;

        public int IdAsignatura { get; set; }
        public string Convocatoria { get; set; }
        public int NumeroMatriculas { get; set; }
        public string NotaNumerica { get; set; }
        public int Estado { get; set; }
        public string DesEstado { get; set; }
        public string AnyoAcademico { get; set; }

        public string Ects { get; set; }
        public string NotaAlfaNumerica { get; set; }
        public bool Superado { get; set; }
        public List<ResponseReconocimientosOrigenModel> ReconocimientosOrigen { get; set; }

        // Lectura
        public bool EsEstadoMatriculada => Estado == EstadoAsignaturaExpedienteMatriculada;
        public bool EsEstadoNoPresentado => Estado == EstadoAsignaturaExpedienteNoPresentado;
        public bool EsEstadoSuspendida => Estado == EstadoAsignaturaExpedienteSuspendida;
        public bool EsEstadoSuperada => Estado == EstadoAsignaturaExpedienteSuperada;
        public bool EsEstadoMatriculaHonor => Estado == EstadoAsignaturaExpedienteMatriculaDeHonor;
        public bool EsEstadoProcesoReconocimiento => Estado == EstadoAsignaturaExpedienteEnProcesoReconocimiento;
        public bool EsEstadoReconocida => Estado == EstadoAsignaturaExpedienteReconocida;

        /// <summary>
        /// Es asignatura Mapeada si existe la asignatura en el gestor mapeo
        /// </summary>
        public bool EsAsignaturaMapeada => IdAsignatura != AsignaturaNoMapeada;
        /// <summary>
        /// Obtiene el nombre del estado de asignatura segun la propiedad Estado
        /// </summary>
        public string DisplayNameEstadoAsignatura
        {
            get
            {
                return Estado switch
                {
                    EstadoAsignaturaExpedienteMatriculada => NombreEstadoMatriculada,
                    EstadoAsignaturaExpedienteNoPresentado => NombreEstadoNoPresentado,
                    EstadoAsignaturaExpedienteSuspendida => NombreEstadoSuspendida,
                    EstadoAsignaturaExpedienteSuperada => NombreEstadoSuperada,
                    EstadoAsignaturaExpedienteMatriculaDeHonor => NombreEstadoMatriculaDeHonor,
                    EstadoAsignaturaExpedienteEnProcesoReconocimiento => NombreEstadoEnProcesoReconocimiento,
                    EstadoAsignaturaExpedienteReconocida => NombreEstadoReconocida,
                    _ => string.Empty
                };
            }
        }
    }
}
