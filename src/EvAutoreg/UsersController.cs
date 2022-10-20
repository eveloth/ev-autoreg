using DataAccessLibrary.Models;
using DataAccessLibrary.Repositories;
using EvAutoreg.Dto;
using EvAutoreg.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvAutoreg
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userRepository.GetAllUsers());
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);

            return user is null ? NotFound("User not found.") : Ok(user);
        }

        [Route("{email}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);

            return user is null ? NotFound("User not found.") : Ok(user);
            
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(UserForCreationDto request)
        {
            var email = request.Email.ToLower();

            var doesUserExist = await _userRepository.DoesUserExist(email);

            if (doesUserExist) return BadRequest("User already exists.");
            
                /*var existingUser = await _userRepository.GetUserByEmail(email);
    
                if (existingUser is not null)
                {
                    return BadRequest("User already exists.");
                }*/

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var newUser = new UserModel
            {
                Email = email,
                PasswordHash = passwordHash
            };

            await _userRepository.CreateUser(newUser);

            return Ok(newUser.Email);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser(UserModel user)
        {
            var existingUser = await _userRepository.GetUserById(user.Id);

            if (existingUser is null) return NotFound("User not found");
            
            await _userRepository.UpdateUser(user);
            return Ok($"User {user.Email} was updated.");
        }

        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var existingUser = await _userRepository.GetUserById(id);

            if (existingUser is null) return NotFound("User not found");
            
            await _userRepository.DeleteUser(id);
            return Ok($"User {existingUser.Email} was deleted.");
        }
    }
}