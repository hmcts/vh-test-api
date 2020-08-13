using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApi.Contract.Responses;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Mappings;
using TestApi.Services.Contracts;

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
        private readonly IUserApiService _userApiService;

        public UserController(IQueryHandler queryHandler, ILogger<UserController> logger,
            IUserApiService userApiService)
        {
            _queryHandler = queryHandler;
            _logger = logger;
            _userApiService = userApiService;
        }

        /// <summary>
        ///     Get user by username
        /// </summary>
        /// <param name="username">Username of the user (case insensitive)</param>
        /// <returns>Full details of a user</returns>
        [HttpGet("username/{username}")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserDetailsByUsernameAsync(string username)
        {
            _logger.LogDebug($"GetUserDetailsByUsernameAsync {username}");

            try
            {
                var queriedUser =
                    await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));
                var response = UserToDetailsResponseMapper.MapToResponse(queriedUser);
                return Ok(response);
            }
            catch (UserNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        ///     Delete AAD user
        /// </summary>
        /// <param name="contactEmail">Email of the user to delete</param>
        /// <returns>Details of the user to delete</returns>
        [HttpDelete("aad")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteADUserAsync(string contactEmail)
        {
            _logger.LogDebug("DeleteADUserAsync");

            if (await _userApiService.CheckUserExistsInAAD(contactEmail))
                await _userApiService.DeleteUserInAAD(contactEmail);
            else
                return NotFound(contactEmail);

            _logger.LogDebug($"User with contact email {contactEmail} deleted");

            return NoContent();
        }
    }
}