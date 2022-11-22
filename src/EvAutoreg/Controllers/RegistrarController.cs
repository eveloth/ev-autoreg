using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RegistrarController : ControllerBase
{
}