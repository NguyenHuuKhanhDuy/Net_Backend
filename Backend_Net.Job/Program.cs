using Backend_Net.Job.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services
    .AddServiceInvocation(builder.Configuration)
    .AddScheduledJobs(builder.Configuration)
    .AddCustomOptions(builder.Configuration);

var host = builder.Build();
host.Run();