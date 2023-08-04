namespace Unir.Expedientes.WebUi.Model.HealthCheck
{
    public class HealthCheck
    {
        /// <summary>
        /// System name been checked.
        /// </summary>
        public string System { get; set; }
        /// <summary>
        /// Check name. Summarized the check been performed.
        /// </summary>
        public string CheckName { get; set; }
        public bool Result { get; set; }
        /// <summary>
        /// Description of error if result was not OK. Null if Ok
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// Total Elapsed Miliseconds to track timeouts on checks.
        /// </summary>
        public double ElapsedMs { get; set; }
    }
}
