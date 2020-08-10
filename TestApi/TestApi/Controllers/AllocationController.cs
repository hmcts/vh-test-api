using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestApi.Contract.Requests;
using TestApi.Contract.Responses;
using TestApi.DAL.Commands;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;
using TestApi.Mappings;
using TestApi.Validations;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("allocations")]
    [ApiController]
    public class AllocationController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<AllocationController> _logger;

        public AllocationController(ICommandHandler commandHandler, IQueryHandler queryHandler, ILogger<AllocationController> logger)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
        }

        /// <summary>
        /// Allocate single user by user type and application
        /// </summary>
        /// <param name="userType">Type of user (e.g Judge)</param>
        /// <param name="application">Application (e.g. VideoWeb)</param>
        /// <returns>Full details of an allocated user</returns>
        [HttpPut]
        [ProducesResponseType(typeof(UserDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateUserByUserTypeAndApplicationAsync(UserType userType, Application application)
        {
            _logger.LogDebug($"AllocateUserByUserTypeAndApplicationAsync {userType} {application}");

            var user = await AllocateAsync(userType, application);
            _logger.LogDebug($"User '{user.Username}' successfully allocated");

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            return Ok(response);
        }

        /// <summary>
        /// Allocate users by user types and application
        /// </summary>
        /// <param name="request">Allocate users request</param>
        /// <returns>Full details of an allocated users</returns>
        [HttpPut("allocateUsers")]
        [ProducesResponseType(typeof(List<UserDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateUsersAsync(AllocateUsersRequest request)
        {
            _logger.LogDebug($"AllocateUsersAsync No. of UserTypes: {request.UserTypes.Count} Application: {request.Application}");

            if (request.UserTypes.Count.Equals(0))
            {
                return BadRequest("Must be more than 1 user type");
            }

            var responses = new List<UserDetailsResponse>();

            foreach (var userType in request.UserTypes)
            {
                var user = await AllocateAsync(userType, request.Application);
                _logger.LogDebug($"User '{user.Username}' successfully allocated");
                responses.Add(UserToDetailsResponseMapper.MapToResponse(user));
            }

            return Ok(responses);
        }

        /// <summary>
        /// Unallocate users by username
        /// </summary>
        /// <param name="request">List of usernames to unallocate</param>
        /// <returns>Allocation details of the unallocated users</returns>
        [HttpPut("unallocateUsers")]
        [ProducesResponseType(typeof(List<AllocationDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnallocateUsersByUsernameAsync(UnallocateUsersRequest request)
        {
            _logger.LogDebug($"UnallocateUsersByUsernameAsync");

            var allocationIds = new List<Guid>();

            foreach (var username in request.Usernames)
            {
                if (!username.IsValidEmail())
                {
                    ModelState.AddModelError(nameof(username), $"Please provide a valid {nameof(username)}");
                    return BadRequest(ModelState);
                }

                var user = await GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound();
                }

                allocationIds.Add(await UnallocateAsync(username));
            }

            var response = new List<AllocationDetailsResponse>();

            foreach (var allocationId in allocationIds)
            {
                var allocation = await GetAllocationByAllocationIdAsync(allocationId);
                response.Add(AllocationToDetailsResponseMapper.MapToResponse(allocation));
            }

            _logger.LogInformation($"Allocated {response.Count} user(s)");

            return Ok(response);
        }

        private async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));
        }

        private async Task<Allocation> GetAllocationByAllocationIdAsync(Guid allocationId)
        {
            return await _queryHandler.Handle<GetAllocationByIdQuery, Allocation>(new GetAllocationByIdQuery(allocationId));
        }

        private async Task<User> AllocateAsync(UserType userType, Application application, int expiresInMinutes = 10)
        {
            var allocateCommand = new AllocateByUserTypeCommand(userType, application, expiresInMinutes);
            await _commandHandler.Handle(allocateCommand);
            return allocateCommand.User;
        }

        private async Task<Guid> UnallocateAsync(string username)
        {
            var unallocateCommand = new UnallocateByUsernameCommand(username);
            await _commandHandler.Handle(unallocateCommand);
            return unallocateCommand.AllocationId;
        }
    }
}
