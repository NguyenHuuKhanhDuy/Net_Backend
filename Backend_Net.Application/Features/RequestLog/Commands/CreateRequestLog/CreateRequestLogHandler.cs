using Backend_Net.Application.Common.Helpers;
using Backend_Net.Application.Common.Interfaces;
using Backend_Net.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Backend_Net.Application.Features.RequestLog.Commands.CreateRequestLog;

public class CreateRequestLogHandler : IRequestHandler<CreateRequestLogCommand>
{
    private readonly ILogger<CreateRequestLogHandler> _logger;
    private readonly IUnitOfWork _uow;
    private readonly ICustomHttpContextAccessor _accessor;

    public CreateRequestLogHandler
    (
        ILogger<CreateRequestLogHandler> logger,
        IUnitOfWork uow,
        ICustomHttpContextAccessor accessor
    )
    {
        _logger = logger;
        _uow = uow;
        _accessor = accessor;
    }

    #region Implementation of IRequestHandler<in CreateRequestLogCommand>

    public async Task Handle(CreateRequestLogCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateRequestLogHandler)} =>";
        _logger.LogInformation(
            "{functionName:l} Handling CreateRequestLogCommand for RequestId: {RequestId:l}, Payload: {payload:l}",
            functionName, payload.RequestId, JsonHelper.Serialize(payload));

        var tenantCredential = await _uow.TenantCredential
            .Where(x => x.ApiKey == _accessor.GetApiKey())
            .Include(x => x.Tenant)
            .AsNoTracking()
            .Select(x => new { Id = x.Id })
            .FirstOrDefaultAsync(cancellationToken);
        if (tenantCredential is null)
        {
            _logger.LogWarning(
                "{functionName:l} Tenant with Id: {TenantId:l} does not exist. Aborting request log creation.",
                functionName, _accessor.GetApiKey());
            return;
        }
        
        var entity = new Domain.Entities.RequestLog
        {
            Id = Guid.NewGuid(),
            TenantCredentialId = tenantCredential.Id,
            Path = payload.Path,
            Method = payload.Method,
            Headers = payload.Headers,
            Body = payload.Body,
            QueryParams = payload.QueryParams,
            ResponseBody = payload.ResponseBody,
            ResponseStatus = payload.ResponseStatus,
            DurationMs = payload.DurationMs,
            IpAddress = payload.IpAddress,
            UserAgent = payload.UserAgent,
            ErrorMessage = payload.ErrorMessage,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.RequestLog.Add(entity);
        await _uow.SaveAsync();
    }

    #endregion
}