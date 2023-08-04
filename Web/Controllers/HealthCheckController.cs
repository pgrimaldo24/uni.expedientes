using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Persistence.Context;
using Unir.Expedientes.WebUi.Middleware.BasicAuth;
using Unir.Expedientes.WebUi.Model.HealthCheck;
using Unir.Framework.Crosscutting.DataCache;
using Unir.Framework.Crosscutting.Files;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/health-check")]
    [BasicAuth("Expedientes Health")]
    [ApiController]
    public class HealthCheckController : ControllerSuperType
    {
        private const string FileBase64 =
               "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AACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADiBCAAAACAOAEIAAAAIE4AAgAAAIgTgAAAAADi/gbTK8s+2c2ZVgAAAABJRU5ErkJggg==";
        private readonly IConfiguration _configuration;
        private readonly IFileManager _fileManager;
        private readonly ExpedientesContext _context;
        private readonly IDataCacheService _dataCacheService;
        private readonly ISecurityIntegrationServices _securityIntegrationServices;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(IConfiguration configuration, IFileManager fileManager,
            ExpedientesContext context, IDataCacheService dataCacheService, ISecurityIntegrationServices securityIntegrationServices,
            IErpAcademicoServiceClient erpAcademicoServiceClient, ILogger<HealthCheckController> logger)
        {
            _configuration = configuration;
            _fileManager = fileManager;
            _context = context;
            _dataCacheService = dataCacheService;
            _securityIntegrationServices = securityIntegrationServices;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> HealthCheck()
        {
            var assembly = GetAssembly();
            var assemblyInfo = assembly.GetName();
            var checks = await GetChecks();
            return Ok(new Health
            {
                Name = assemblyInfo.Name,
                Healthy = checks.TrueForAll(c => c.Result),
                Version = assemblyInfo.Version?.ToString() ?? "",
                Descripcion = GetDescription(assembly),
                Timestamp = DateTime.UtcNow,
                Checks = checks,
                Metadata = GetMetaData()
            });
        }
        private static Assembly GetAssembly()
        {
            return typeof(HealthCheckController).Assembly;
        }

        private static string GetDescription(Assembly assembly)
        {
            var attribute = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault();
            var description = (attribute as AssemblyDescriptionAttribute)?.Description;
            return description ?? "N/A";
        }

        private static IList<KeyValuePair<string, string>> GetMetaData()
        {
            var process = Process.GetCurrentProcess();
            var totalSizeKb = GetTotalSizeKb();
            var processWorkingSet = process.WorkingSet64 / 1024;
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Host", Dns.GetHostName()),
                new KeyValuePair<string, string>("PID", process.Id.ToString()),
                GetWorkingSet64(process),
                GetPeakWorkingSet64Kb(process),
                new KeyValuePair<string, string>("Memory.AllProcessTotalSizeKb",
                    totalSizeKb.ToString(CultureInfo.InvariantCulture)),
                new KeyValuePair<string, string>("Memory.Usage",
                    (processWorkingSet / totalSizeKb).ToString(CultureInfo.InvariantCulture) + "%"),
                GetTotalProcessorTimeSeconds(process)
            };
        }

        /// <param name="process"></param>
        /// <returns>La cantidad de memoria física, en bytes, asignada para el proceso asociado.</returns>
        private static KeyValuePair<string, string> GetWorkingSet64(Process process)
        {
            return new KeyValuePair<string, string>("Memory.WorkingSet64Kb", (process.WorkingSet64 / 1024).ToString());
        }

        /// <param name="process"></param>
        /// <returns>La cantidad máxima de memoria física, en bytes, asignada para el proceso asociado desde que se inició.</returns>
        private static KeyValuePair<string, string> GetPeakWorkingSet64Kb(Process process)
        {
            return new KeyValuePair<string, string>("Memory.PeakWorkingSet64Kb",
                (process.PeakWorkingSet64 / 1024).ToString());
        }

        /// <param name="process"></param>
        /// <returns>Un intervalo que indica la cantidad de tiempo que el proceso asociado ha pasado utilizando la CPU.</returns>
        private static KeyValuePair<string, string> GetTotalProcessorTimeSeconds(Process process)
        {
            return new KeyValuePair<string, string>("CPU.TotalProcessorTimeSeconds",
                process.TotalProcessorTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
        }

        /// <returns>La cantidad en bytes, de todos los procesos</returns>
        private static double GetTotalSizeKb()
        {
            return Process.GetProcesses().Sum(aProc => aProc.WorkingSet64 / 1024.0);
        }

        #region Checks

        private async Task<List<HealthCheck>> GetChecks()
        {
            return new List<HealthCheck>
            {
                Ping(),
                await UploadSystemFile(),
                await UploadFileManager(),
                await ConnectionDatabase(),
                await GetTokenGestorDocumental(),
                await GetTokenErpAcademico(),
                await GetTokenOAuthServices(),
                await IntegracionErpAcademico(),
                await DataCacheServices(),
                Logger()
            };
        }

        private static HealthCheck Ping()
        {
            var timeInicial = DateTime.UtcNow;
            var check = new HealthCheck
            {
                System = "Ping",
                CheckName = "Connectivity",
                Result = true
            };
            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private HealthCheck Logger()
        {
            var check = new HealthCheck
            {
                System = "Logger",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                _logger.LogError("LogError");
                _logger.LogInformation("LogInformation");
                _logger.LogWarning("LogWarning");
                _logger.LogDebug("LogDebug");
                check.Result = true;
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private static double GetElapsedMs(DateTime timeInicial, DateTime timeFinal)
        {
            return Math.Round((timeFinal - timeInicial).TotalMilliseconds, 2, MidpointRounding.AwayFromZero);
        }

        private async Task<HealthCheck> UploadSystemFile()
        {
            var contenidoArchivo = Convert.FromBase64String(FileBase64);
            var rutaAbsoluta = _configuration.GetValue<string>("Files:AbsolutePath");

            var check = new HealthCheck
            {
                System = "System.IO.File",
                CheckName = "UploadFile"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var outputDirectory = Path.Combine(rutaAbsoluta, "test");
                Directory.CreateDirectory(outputDirectory);
                var outputFile = Path.Combine(rutaAbsoluta, "test", "test.png");
                await System.IO.File.WriteAllBytesAsync(outputFile, contenidoArchivo);
                var result = System.IO.File.Exists(outputFile);
                check.Result = result;
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> UploadFileManager()
        {
            var contenidoArchivo = Convert.FromBase64String(FileBase64);
            var check = new HealthCheck
            {
                System = "FileManager",
                CheckName = "UploadFile"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var outPutDirectory = Path.Combine("test", "testFileManager.png");
                await _fileManager.WriteFileAsync(outPutDirectory, contenidoArchivo);
                var result = _fileManager.Exist(outPutDirectory);
                check.Result = result;
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> ConnectionDatabase()
        {
            var check = new HealthCheck
            {
                System = "Context.DataBase",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var resultConnection = await _context.Database.CanConnectAsync(CancellationToken.None);
                check.Result = resultConnection;
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> GetTokenGestorDocumental()
        {
            var check = new HealthCheck
            {
                System = "Integracion.GestorDocumental",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var resultToken = await _securityIntegrationServices.GetTokenGestorDocumental();
                check.Result = !string.IsNullOrEmpty(resultToken);
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> GetTokenErpAcademico()
        {
            var check = new HealthCheck
            {
                System = "Integracion.Erp.Token",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var resultToken = await _securityIntegrationServices.GetTokenErpAcademico();
                check.Result = !string.IsNullOrEmpty(resultToken);
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> GetTokenOAuthServices()
        {
            var check = new HealthCheck
            {
                System = "Integracion.Auth.Token",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var resultToken = await _securityIntegrationServices.GetTokenServiciosDesUnirExpedientes();
                check.Result = !string.IsNullOrEmpty(resultToken);
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> IntegracionErpAcademico()
        {
            var parameters = new UniversidadListParameters
            {
                Count = 1,
                Index = 1
            };
            var check = new HealthCheck
            {
                System = "Integracion.Erp.GetUniversidades",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                var centros =
                    await _erpAcademicoServiceClient.GetUniversidades(parameters);
                check.Result = centros.Any();
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        private async Task<HealthCheck> DataCacheServices()
        {
            const string key = "DATA_CACHE_SERVICES";
            var check = new HealthCheck
            {
                System = "DataCacheServices",
                CheckName = "Connectivity"
            };
            var timeInicial = DateTime.UtcNow;
            try
            {
                await _dataCacheService.PutAsync(key, Guid.NewGuid().ToString(), TimeSpan.FromSeconds(30));
                var cache = await _dataCacheService.GetAsync<string>(key);
                check.Result = !string.IsNullOrEmpty(cache);
            }
            catch (Exception ex)
            {
                check.Error = ex.Message;
                check.Result = false;
            }

            var timeFinal = DateTime.UtcNow;
            check.ElapsedMs = GetElapsedMs(timeInicial, timeFinal);
            return check;
        }

        #endregion
    }
}
