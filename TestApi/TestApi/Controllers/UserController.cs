using System;
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

            var user = await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));

            if (user == null)
            {
                return NotFound();
            }

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            return Ok(response);
        }

        /// <summary>
        ///     Check if user exists in AAD by contact email
        /// </summary>
        /// <param name="contactEmail">Contact email of the user (case insensitive)</param>
        /// <returns>True if user exists, false if not</returns>
        [HttpGet("aad/{contactEmail}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUserExistsInAdAsync(string contactEmail)
        {
            _logger.LogDebug($"GetUserExistsInAdAsync {contactEmail}");

            var exists = await _userApiService.CheckUserExistsInAAD(contactEmail);

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