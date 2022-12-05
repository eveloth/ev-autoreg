using Microsoft.AspNetCore.SignalR;

namespace Autoregistrar.Hubs;

public class AutoregistrarHub : Hub<IAutoregistrarClient>
{
}