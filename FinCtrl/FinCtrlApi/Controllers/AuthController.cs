using FinCtrlApi.Databases.MongoDb.FinCtrl.Interfaces;
using FinCtrlLibrary.Models;
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
                    return NotFound("Email e/ou senha incorretos! Verifique e tente novamente.");

                string? currentIp = GetIpFromRequest(Request.HttpContext.Connection.RemoteIpAddress);

                if (string.IsNullOrEmpty(currentIp))
                    return BadRequest("Não foi possível obter o IP do cliente."); 

                Authentication authentication = await _authenticatonRepo.GetUserAuthenticationAsync(databaseUser, currentIp);

                return Ok(authentication);
            }
            catch (Exception ex)
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
