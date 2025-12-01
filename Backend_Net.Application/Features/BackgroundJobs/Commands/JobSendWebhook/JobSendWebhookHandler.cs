using Backend_Net.Application.Common.Extensions;
using Backend_Net.Application.Common.Interfaces.MassTransit;
using Backend_Net.Application.Common.Interfaces.Repositories;
using Backend_Net.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared.MassTransit.Contracts.Queues;
using Shared.MassTransit.IntegrationEvents;

namespace Backend_Net.Application.Features.BackgroundJobs.Commands.JobSendWebhook;

public class JobSendWebhookHandler : IRequestHandler<JobSendWebhookCommand>
{
    private readonly ILogger<JobSendWebhookHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageSender _messageSender;
    
    private const int ChunkSize = 100;

    public JobSendWebhookHandler
    (
        ILogger<JobSendWebhookHandler> logger,
        IUnitOfWork unitOfWork,
        IMessageSender messageSender
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _messageSender = messageSender;
    }

    #region Implementation of IRequestHandler<in SendWebhookCommand, SendWebhookResponse>

    public async Task Handle(JobSendWebhookCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(JobSendWebhookHandler)} =>";
        var now = DateTime.UtcNow;
        _logger.LogInformation("{functionName:l} Started at {now:O}", functionName, now);
        
        try
        {
            _logger.LogInformation("{functionName:l} Step 1: Get webhook delivery", functionName);
            var items = await _unitOfWork.WebhookDelivery
                .Where(x => (x.Status == WebhookDeliveryStatus.Pending || x.Status == WebhookDeliveryStatus.Failed)
                            && (x.RetryAt == null || x.RetryAt <= now))
                .OrderBy(x => x.CreatedAt)
                .AsNoTracking()
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("{functionName:l} Step 2: Found {count} webhook deliveries to process", functionName, items.Count);
            if (!items.Any())
            {
                return;
            }
            
            var chunks = items.Chunk(ChunkSize);
            _logger.LogInformation("{functionName:l} Step 2: Processing {chunkCount} chunks of size {chunkSize}", functionName, chunks.Count(), ChunkSize);

            Parallel.ForEach(chunks, chunk =>
            {
                _messageSender.SendMessage<SendWebhook>(new
                {
                    Content = new SendWebhookEvent()
                    {
                        WebhookDeliveryIds = chunk.ToList()
                    }
                }, cancellationToken).FireAndForget();
            });
            
            _logger.LogInformation("{functionName:l} Completed at {now:O}", functionName, DateTime.UtcNow);
        }
        catch (Exception e)
        {
            e.LogError(_logger, functionName);
        }
    }

    #endregion
}