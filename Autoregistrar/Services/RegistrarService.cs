using Grpc.Core;

namespace Autoregistrar.Services;

public class RegistrarService : Registrar.RegistrarBase
{
    public override Task<StatusResponse> StartService(StartRequest request, ServerCallContext context)
    {
        return Task.FromResult(new StatusResponse {Message = "Service started"});
    }
}