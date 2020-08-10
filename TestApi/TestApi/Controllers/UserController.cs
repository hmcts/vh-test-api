using System.Net;
using System.Threading.Tasks;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.DAL.Commands;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApi.Services.Clients.UserApiClient;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<UserController> _logger;

        public UserController(ICommandHandler commandHandler, IQueryHandler queryHandler, ILogger<UserController> logger)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username of the user (case insensitive)</param>
        /// <returns>Full details of a user</returns>
        [HttpGet("username/{username}")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserDetailsByUsernameAsync(string username)
        {
            _logger.LogDebug($"GetUserDetailsByUsernameAsync {username}");

            var queriedUser = await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));

            if (queriedUser == null)
            {
                _logger.LogWarning($"Unable to find user with username {username}");
                return NotFound();
            }

            var response = UserToDetailsResponseMapper.MapToResponse(queriedUser);
            return Ok(response);
        }

        /// <summary>
        /// Delete AAD user
        /// </summary>
        /// <param name="contactEmail">Email of the user to delete</param>
        /// <returns>Details of the user to delete</returns>
        [HttpDelete("aad")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteADUserAsync(string contactEmail)
        {
            _logger.LogDebug("CreateNewADUser");


            await _commandHandler.Handle(createNewAdUserCommand);

            var user = createNewAdUserCommand.Response;
            _logger.LogDebug($"New User with username {user.Username} Created");

            return CreatedAtAction(nameof(CreateNewADUserAsync), new { userId = createNewAdUserCommand.Response.User_id }, createNewAdUserCommand.Response);
        }
    }
}
