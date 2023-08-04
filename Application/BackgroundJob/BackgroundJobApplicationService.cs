using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Hub;
using Unir.Framework.Crosscutting.BackgroundJob;
using System;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.BackgroundJob
{
    public class BackgroundJobApplicationService : IBackgroundJobApplicationService
    {
        protected readonly IExpedientesContext _context;
        protected readonly IMediator _mediator;
        protected readonly ILogger<BackgroundJobApplicationService> _logger;
        protected readonly IBackgroundJobService _backgroundJobService;

        protected IHubContext<ExpedientesHub, IExpedientesClient> _expedientesHubContext { get; }

        public BackgroundJobApplicationService(
            IExpedientesContext context,
            IMediator mediator,
            ILogger<BackgroundJobApplicationService> logger,
            IBackgroundJobService backgroundJobService,
            IHubContext<ExpedientesHub, IExpedientesClient> expedientesHubContext)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
            _backgroundJobService = backgroundJobService;
            _expedientesHubContext = expedientesHubContext;
        }

        protected internal virtual async Task<string> ExecuteSendMediator(IRequest request, string message, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
            }
            catch (BadRequestException ex)
            {
                return $"{message} Causa: '{ex.Message}'";
            }
            catch (NotFoundException ex)
            {
                return $"{message} Causa: '{ex.Message}'";
            }
            catch (ValidationErrorsException ex)
            {
                return $"{message} Causa: '{ex.Message}', Detalles: '{string.Join(',', ex.Details)}'";
            }
            catch (Exception ex)
            {
                return $"{message} Causa: '{ex.Message}'";
            }
            return null;
        }

        public JobDetails GetJobDetails(string procesoId)
        {
            return _backgroundJobService.GetJobDetails(procesoId);
        }
    }
}
