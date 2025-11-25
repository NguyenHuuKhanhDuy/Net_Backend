using MediatR;

namespace Backend_Net.Application.Features.RequestLog.Commands.CreateRequestLog;

public class CreateRequestLogCommand : IRequest
{
    public CreateRequestLogRequest Payload { get; set; }

    public CreateRequestLogCommand(CreateRequestLogRequest payload)
    {
        Payload = payload;
    }
}