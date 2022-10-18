using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserManagementController(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userRepository.GetAllUsers());
        }

        [Route("users/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);

            return user is null ? NotFound("User not found.") : Ok(user);
        }

        [Route("users/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            return user is null ? NotFound("User not found.") : Ok(user);
            
        }

        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserDto request)
        {
            var email = request.Email.ToLower();
            var existingUser = await _userRepository.GetUserByEmail(email);

            if (existingUser is not null)
            {
                return BadRequest("User already exists.");
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var newUser = new UserModel
            {
                Email = email,
                PasswordHash = passwordHash
            };

            await _userRepository.CreateUser(newUser);

            return Ok(newUser.Email);
        }
    }
}
