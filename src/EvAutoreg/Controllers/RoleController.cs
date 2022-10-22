using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IUserRepository _userRepository;

        public RoleController(IUserRolesRepository userRolesRepository, IUserRepository userRepository)
        {
            _userRolesRepository = userRolesRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userRolesRepository.GetRoles();

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleDto role)
        {
            var roleName = role.RoleName.ToLower();
            
            await _userRolesRepository.AddRole(roleName);

            return Ok($"{roleName} role was added.");
        }

        [Route("user-role")]
        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var userRoles = await _userRolesRepository.GetUserRoles();

            return Ok(userRoles);
        }

        [Route("user-role")]
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(UserRoleDto userRole)
        {
            var roleName = userRole.RoleName.ToLower();
            
            var userExists = await _userRepository.DoesUserExist(userRole.UserId);

            if (!userExists)
            {
                return NotFound("User doesn't exist");
            }

            var roleExists = await _userRolesRepository.DoesRoleExist(userRole.RoleName);

            if (!roleExists)
            {
                return NotFound("Role doesn't exist.");
            }

            var isOperationSuccessfull = await _userRolesRepository.SetUserRole(userRole.UserId, roleName);

            if (!isOperationSuccessfull)
            {
                return BadRequest("Something went wrong.");
            }

            return Ok("Successfully added user to role");
        }
    }
}
