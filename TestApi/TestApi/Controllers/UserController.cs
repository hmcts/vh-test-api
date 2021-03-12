using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Mappings;
using TestApi.Services.Services;
using UserApi.Client;
using UserApi.Contract.Responses;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IQueryHandler _queryHandler;
        private readonly IUserApiClient _userApiClient;
        private readonly IUserApiService _userApiService;

        public UserController(IQueryHandler queryHandler, ILogger<UserController> logger,
            IUserApiService userApiService, IUserApiClient userApiClient)
        {
            _queryHandler = queryHandler;
            _logger = logger;
            _userApiService = userApiService;
            _userApiClient = userApiClient;
        }

        /// <summary>
        ///     Get test api user by username
        /// </summary>
        /// <param name="username">Username of the user (case insensitive)</param>
        /// <returns>Full details of a user</returns>
        [HttpGet("username/{username}")]
        [OpenApiOperation("GetUserDetailsByUsername")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserDetailsByUsername(string username)
        {
            _logger.LogDebug("GetUserDetailsByUsername {username}", username);

            var user = await _queryHandler.Handle<GetUserByUsernameQuery, UserDto>(new GetUserByUsernameQuery(username));

            if (user == null)
            {
                return NotFound();
            }

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            return Ok(response);
        }

        /// <summary>
        ///     Get user by user principal name
        /// </summary>
        /// <param name="username">Username of the user (case insensitive)</param>
        /// <returns>Full details of a user</returns>
        [HttpGet("userPrincipalName/{username}")]
        [OpenApiOperation("GetUserByUserPrincipleName")]
        [ProducesResponseType(typeof(UserProfile), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserByUserPrincipleName(string username)
        {
            _logger.LogDebug("GetUserByUserPrincipleName {username}", username);

            try
            {
                var response = await _userApiClient.GetUserByAdUserNameAsync(username);
                return Ok(response);
            }
            catch (UserApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Check if user exists in AAD by username
        /// </summary>
        /// <param name="username">Username of the user (case insensitive)</param>
        /// <returns>True if user exists, false if not</returns>
        [HttpGet("aad/{username}")]
        [OpenApiOperation("GetUserExistsInAd")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserExistsInAd(string username)
        {
            _logger.LogDebug("GetUserExistsInAd {username}", username);

            var exists = await _userApiService.CheckUserExistsInAAD(username);

            if (exists)
            {
                return Ok(true);
            }

            return NotFound(false);
        }

        /// <summary>
        ///     Delete AAD user
        /// </summary>
        /// <param name="contactEmail">Email of the user to delete</param>
        /// <returns>Details of the user to delete</returns>
        [HttpDelete("aad/{contactEmail}")]
        [OpenApiOperation("DeleteADUser")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteADUser(string contactEmail)
        {
            _logger.LogDebug("DeleteADUser");

            if (await _userApiService.CheckUserExistsInAAD(contactEmail))
                await _userApiService.DeleteUserInAAD(contactEmail);
            else
                return NotFound(contactEmail);

            _logger.LogDebug("User with contact email {contactEmail} deleted", contactEmail);

            return NoContent();
        }

        /// <summary>
        ///     Refresh Judges Cache
        /// </summary>
        [HttpGet("judges/cache")]
        [OpenApiOperation("RefreshJudgesCache")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RefreshJudgesCache()
        {
            _logger.LogDebug($"RefreshJudgesCache");

            try
            {
                await _userApiClient.RefreshJudgeCacheAsync();
                return Ok();
            }
            catch (UserApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }

        /// <summary>
        ///     Reset user password
        /// </summary>
        /// <param name="request">Details of the required user</param>
        [HttpPatch("aad/password")]
        [OpenApiOperation("ResetUserPassword")]
        [ProducesResponseType(typeof(UpdateUserResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ResetUserPassword(ResetUserPasswordRequest request)
        {
            _logger.LogDebug($"ResetUserPassword");

            try
            {
                var response = await _userApiClient.ResetUserPasswordAsync(request.Username);
                return Ok(response);
            }
            catch (UserApiException e)
            {
                return StatusCode(e.StatusCode, e.Response);
            }
        }
    }
}