using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
using FinCtrlLibrary.Models.StaticModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

namespace FinCtrlApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationRepository _authenticatonRepo;
        private readonly IUserRepository _userRepo;

        public AuthController(IAuthenticationRepository authenticationRepository, IUserRepository userRepository)
        {
            _authenticatonRepo = authenticationRepository;
            _userRepo = userRepository;
        }

        [HttpPost, Route("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] User user)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.ToString());

            try
            {
                User databaseUser = await _userRepo.GetByEmailAndPasswordAsync(user.Email, user.Password);

                if (databaseUser == null)
                    return NotFound("Incorrect email and/or password! Verify and try again.");

                string? currentIp = GetIpFromRequest(Request.HttpContext.Connection.RemoteIpAddress);

                if (string.IsNullOrEmpty(currentIp))
                    return BadRequest("Unable to obtain the request IP."); 

                Authentication authentication = await _authenticatonRepo.GetUserAuthenticationAsync(databaseUser, currentIp);

                databaseUser.LastLogin = DateTime.UtcNow;
                await _userRepo.UpdateAsync(databaseUser);

                return Ok(authentication);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [HttpPost, Route("CreateCommomUser")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToString());

            try
            {
                user.ValidateForCreation();

                if (!user.IsValid)
                    return BadRequest(user.Errors);

                bool alreadyExistsUser = await _userRepo.CheckIfAlreadyExistsUserByEmail(user.Email);

                if (alreadyExistsUser)
                    return BadRequest("It already exists an user with the informed Email!");

                user.Role = UserRoles.CommonUser;
                await _userRepo.InsertNewAsync(user);

                return Created();
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public static string GetIpFromRequest(IPAddress requestIpAddress)
        {
            string requestIp = string.Empty;

            if (IPAddress.IsLoopback(requestIpAddress))
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(requestIpAddress);

                foreach (IPAddress ip in hostEntry.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        requestIp = ip.ToString();
                }
            }
            else
                requestIp = requestIpAddress.ToString();

            return requestIp;
        }
    }
}
