using DotNetCoreAdvance.Cache;
using DotNetCoreAdvance.DataContext;
using DotNetCoreAdvance.Interfaces;
using DotNetCoreAdvance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace DotNetCoreAdvance.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLoggerRepository _auditLoggerRepository;
        private readonly ICacheService _cacheService;

        public UserController(IUserRepository userRepository, IAuditLoggerRepository auditLoggerRepository, ICacheService cacheService)
        {
            _userRepository = userRepository;
            _auditLoggerRepository = auditLoggerRepository;
            _cacheService = cacheService;
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(User user)
        {
            if (user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "User can not be null or empty");
            }

            try
            {
                await _cacheService.RemoveData("product");

                user.LastEdited = DateTime.UtcNow;
                await _userRepository.Add(user);

                return StatusCode(StatusCodes.Status200OK, user);
            }
            catch (Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));

                await _auditLoggerRepository.Add(new Audit
                {
                    Module = nameof(UserController),
                    MethodName = nameof(AddUser),
                    RequestObject = JsonConvert.SerializeObject(user),
                    ResponseObject = JsonConvert.SerializeObject(ex),
                    Message = ex.Message,
                    LogLevel = LogLevel.Error
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }

        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser(string userId, EditUserViewModel model)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Id can't be null or empty");
            }

            try
            {
                var isUserExist = await _userRepository.IsUserExist(userId);

                if (isUserExist)
                {
                    var user = await _userRepository.Get(userId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.Gender = model.Gender;
                    user.Skills = model.Skills;
                    user.LastEdited = DateTime.UtcNow;

                    await _cacheService.RemoveData("user");
                    await _userRepository.Update(user);

                    return StatusCode(StatusCodes.Status200OK, "User edited successfully");
                }

                return StatusCode(StatusCodes.Status404NotFound, "User not exist");
            }
            catch(Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));

                await _auditLoggerRepository.Add(new Audit
                {
                    Module = nameof(UserController),
                    MethodName = nameof(EditUser),
                    RequestObject = JsonConvert.SerializeObject(model),
                    ResponseObject = JsonConvert.SerializeObject(ex),
                    Text = $"UserId: {userId}",
                    Message = ex.Message,
                    LogLevel = LogLevel.Error
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Id can't be null or empty");
            }

            try
            {
                await _cacheService.RemoveData("user");
                await _userRepository.Delete(userId);

                return StatusCode(StatusCodes.Status200OK, "User deleted successfully");
            }
            catch(Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));

                await _auditLoggerRepository.Add(new Audit
                {
                    Module = nameof(UserController),
                    MethodName = nameof(DeleteUser),
                    ResponseObject = JsonConvert.SerializeObject(ex),
                    Text = $"UserId: {userId}",
                    Message = ex.Message,
                    LogLevel = LogLevel.Error
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id)) 
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Id can't be null or empty");
            }

            try
            {
                User? user = null;

                var cachedUsers = await _cacheService.GetData<IEnumerable<User>>("user");

                if (cachedUsers != null && cachedUsers.Any())
                {
                    user = cachedUsers.FirstOrDefault(x => x.Id == id);

                    if (user != null)
                    {
                        return StatusCode(StatusCodes.Status200OK, user);
                    }
                }

                user = await _userRepository.Get(id);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, "User not found");
                }

                return StatusCode(StatusCodes.Status200OK, user);
            }
            catch(Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));

                await _auditLoggerRepository.Add(new Audit
                {
                    Module = nameof(UserController),
                    MethodName = nameof(GetUser),
                    ResponseObject = JsonConvert.SerializeObject(ex),
                    Text = $"UserId: {id}",
                    Message = ex.Message,
                    LogLevel = LogLevel.Error
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var cachedUsers = await _cacheService.GetData<IEnumerable<User>>("user");

                if (cachedUsers != null && cachedUsers.Any()) 
                {
                    return StatusCode(StatusCodes.Status200OK, cachedUsers);
                }

                var expirationTime = DateTimeOffset.UtcNow.AddMinutes(4);

                var users = await _userRepository.GetAll();

                await _cacheService.SetData("user", users, expirationTime);

                return StatusCode(StatusCodes.Status200OK, users);
            }
            catch(Exception ex)
            {
                Log.Error(JsonConvert.SerializeObject(ex));

                await _auditLoggerRepository.Add(new Audit
                {
                    Module = nameof(UserController),
                    MethodName = nameof(GetAllUser),
                    ResponseObject = JsonConvert.SerializeObject(ex),
                    Message = ex.Message,
                    LogLevel = LogLevel.Error
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }
}