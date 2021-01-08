using System.Collections.Generic;
using System.Linq;
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
using TestApi.Mappings;

namespace TestApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("allocations")]
    [ApiController]
    public class AllocationController : ControllerBase
    {
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger<AllocationController> _logger;
        private readonly IQueryHandler _queryHandler;

        public AllocationController(ICommandHandler commandHandler, IQueryHandler queryHandler,
            ILogger<AllocationController> logger)
        {
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
            _logger = logger;
        }

        /// <summary>
        ///     Allocate single user
        /// </summary>
        /// <param name="request">Details of the required allocation</param>
        /// <returns>Full details of an allocated user</returns>
        [HttpPatch("allocateUser")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateSingleUserAsync(AllocateUserRequest request)
        {
            _logger.LogDebug($"AllocateSingleUserAsync {request.UserType} {request.Application}");

            var user = await AllocateAsync(request);
            _logger.LogDebug($"User '{user.Username}' successfully allocated");

            var response = UserToDetailsResponseMapper.MapToResponse(user);
            return Ok(response);
        }

        /// <summary>
        ///     Allocate multiple users
        /// </summary>
        /// <param name="request">Allocate users request</param>
        /// <returns>Full details of an allocated users</returns>
        [HttpPatch("allocateUsers")]
        [ProducesResponseType(typeof(List<UserDetailsResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateUsersAsync(AllocateUsersRequest request)
        {
            _logger.LogDebug(
                $"AllocateUsersAsync No. of UserTypes: {request.UserTypes.Count} Application: {request.Application}");

            var responses = new List<UserDetailsResponse>();

            foreach (var userType in request.UserTypes)
            {
                var allocateRequest = new AllocateUserRequest()
                {
                    Application = request.Application,
                    ExpiryInMinutes = request.ExpiryInMinutes,
                    IsProdUser = request.IsProdUser,
                    TestType = request.TestType,
                    UserType = userType
                };

                var user = await AllocateAsync(allocateRequest);
                _logger.LogDebug($"User '{user.Username}' successfully allocated");
                responses.Add(UserToDetailsResponseMapper.MapToResponse(user));
            }

            _logger.LogInformation($"Allocated {responses.Count} user(s)");

            return Ok(responses);
        }

        /// <summary>
        ///     Unallocate users by username
        /// </summary>
        /// <param name="request">List of usernames to unallocate</param>
        /// <returns>Allocation details of the unallocated users</returns>
        [HttpPatch("unallocateUsers")]
        [ProducesResponseType(typeof(List<AllocationDetailsResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnallocateUsersByUsernameAsync(UnallocateUsersRequest request)
        {
            _logger.LogDebug("UnallocateUsersByUsernameAsync");

            var allocations = new List<Allocation>();

            foreach (var username in request.Usernames)
            {
                var user = await GetUserByUsernameAsync(username);

                if (user == null) return NotFound();

                var allocation = await GetAllocationByUsernameAsync(user.Username);

                if (allocation == null)
                    return BadRequest($"No allocation exists for user with username {user.Username}");

                await UnallocateAsync(username);

                allocation = await GetAllocationByUsernameAsync(user.Username);

                allocations.Add(allocation);
            }

            var response = allocations.Select(AllocationToDetailsResponseMapper.MapToResponse).ToList();

            _logger.LogInformation($"Unallocated {response.Count} user(s)");

            return Ok(response);
        }

        /// <summary>
        ///     Get allocated users by allocatedBy
        /// </summary>
        /// <param name="username">Username of the user that has allocated users</param>
        /// <returns>Full details of any allocated users</returns>
        [HttpGet("allocatedUsers/{username}")]
        [ProducesResponseType(typeof(List<AllocationDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllocatedUsersAsync(string username)
        {
            _logger.LogDebug($"GetAllocatedUsersAsync {username}");

            var allocations = await GetAllAllocatedByUsers(username);
            _logger.LogDebug($"Allocations for '{username}' successfully retrieved");

            var responses = allocations.Select(AllocationToDetailsResponseMapper.MapToResponse).ToList();

            _logger.LogInformation($"{responses.Count} user(s) found");

            return Ok(responses);
        }

        private async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));
        }

        private async Task<Allocation> GetAllocationByUsernameAsync(string username)
        {
            return await _queryHandler.Handle<GetAllocationByUsernameQuery, Allocation>(
                new GetAllocationByUsernameQuery(username));
        }

        private async Task<User> AllocateAsync(AllocateUserRequest request)
        {
            return await _queryHandler.Handle<GetAllocatedUserByUserTypeQuery, User>(
                new GetAllocatedUserByUserTypeQuery(request));
        }

        private async Task UnallocateAsync(string username)
        {
            var unallocateCommand = new UnallocateByUsernameCommand(username);
            await _commandHandler.Handle(unallocateCommand);
        }

        private async Task<List<Allocation>> GetAllAllocatedByUsers(string username)
        {
            return await _queryHandler.Handle<GetAllAllocationsForAUserQuery, List<Allocation>>(
                new GetAllAllocationsForAUserQuery(username));
        }
    }
}