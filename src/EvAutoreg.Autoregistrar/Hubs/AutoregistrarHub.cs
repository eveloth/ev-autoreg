using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EvAutoreg.Autoregistrar.Hubs;

[Authorize]
public class AutoregistrarHub : Hub<IAutoregistrarClient>
{
}