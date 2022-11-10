using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static EvAutoreg.Errors.ErrorCodes;

namespace EvAutoreg.Controllers
{
    [AllowAnonymous]
    [Route("api/access-control")]
    [ApiController]
    public class AccessControlController : ControllerBase
    {
        private readonly ILogger<AccessControlController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAccessControlRepository _acRepository;

        public AccessControlController(IAccessControlRepository acRepository, ILogger<AccessControlController> logger, IUserRepository userRepository)
        {
            _acRepository = acRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUs(CancellationToken cts)
        {
            return Ok(await _acRepository.GetAllRolePermissions(cts));
        }

        [Route("user-roles")]
        [HttpPatch]
        public async Task<IActionResult> AddUserToRole(UserRoleDto userRole, CancellationToken cts)
        {
            var userExists = await _userRepository.DoesUserExist(userRole.UserId, cts);

            if (!userExists)
            {
                return NotFound(ErrorCode[2001]);
            }

            var roleExists = await _acRepository.DoesRoleExist(userRole.RoleId, cts);

            if (!roleExists)
            {
                return NotFound(ErrorCode[3001]);
            }

            try
            {
                var updatedUser = await _acRepository.SetUserRole(
                    userRole.UserId,
                    userRole.RoleId,
                    cts
                );
                _logger.LogInformation(
                    "User ID {UserId} was added to role ID {RoleId}",
                    updatedUser.Id,
                    updatedUser.Role!.Id
                );
                return Ok(updatedUser);
            }
            catch (NpgsqlException e)
            {
                _logger.LogError("{ErrorMessage}", e.Message);
                return StatusCode(500, ErrorCode[9001]);
            }
        }
    }
}
