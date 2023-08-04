using MediatR;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.SharedDtos;
using Unir.Framework.Crosscutting.BackgroundJob.Persistence;
using System.Text.Json;
using Unir.Framework.Crosscutting.BackgroundJob;
using Unir.Expedientes.Application.BackgroundJob;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetProcesosEnSegundoPlano
{
    public class GetProcesosEnSegundoPlanoQueryHandler : IRequestHandler<GetProcesosEnSegundoPlanoQuery, ProcesoBackgroundDto[]>
    {
        private readonly IJobExtraInfoRepository _jobExtraInfoRepository;
        private readonly IBackgroundJobApplicationService _backgroundJobApplicationService;
        public GetProcesosEnSegundoPlanoQueryHandler(IJobExtraInfoRepository jobExtraInfoRepository,
            IBackgroundJobApplicationService backgroundJobApplicationService)
        {
            _jobExtraInfoRepository = jobExtraInfoRepository;
            _backgroundJobApplicationService = backgroundJobApplicationService;
        }
        public async Task<ProcesoBackgroundDto[]> Handle(GetProcesosEnSegundoPlanoQuery request, CancellationToken cancellationToken)
        {
            var procesos = _jobExtraInfoRepository.ListJobs()
                .Where(job => _backgroundJobApplicationService.GetJobDetails(job.JobId)?.JobStateDate?.JobState ==
                              JobState.Processing).ToList();
            procesos = procesos.Where(job => job != null).ToList();
            if (!procesos.Any())
                return Array.Empty<ProcesoBackgroundDto>();
            return await Task.Run(() => procesos.Select(job => JsonSerializer.Deserialize<ProcesoBackgroundDto>(
                    JsonSerializer.Serialize(job.CustomData))).ToArray(),
                cancellationToken);
        }
    }
}
