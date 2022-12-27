using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Autoregistrar.Hubs;

[Authorize]
public class AutoregistrarHub : Hub<IAutoregistrarClient>
{
}