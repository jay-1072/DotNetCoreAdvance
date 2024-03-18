using DotNetCoreAdvance.Enums;
using DotNetCoreAdvance.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace DotNetCoreAdvance.Versions.V2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.2")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var expirationTime = DateTimeOffset.UtcNow.AddMinutes(4);

                var users = (await _userRepository.GetAll()).Where(x => x.Gender == Gender.Male);

                return StatusCode(StatusCodes.Status200OK, users);
            }
            catch (Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }
}