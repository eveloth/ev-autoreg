using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EvAutoreg.Autoregistrar.Hubs;

[Authorize(Policy = "UseRegistrar")]
public class AutoregistrarHub : Hub<IAutoregistrarHubClient>
{
}