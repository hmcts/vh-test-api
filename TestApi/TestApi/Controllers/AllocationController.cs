using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using TestApi.Contract.Dtos;
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
        private static readonly object AllocationLock = new object();

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
        [OpenApiOperation("AllocateSingleUser")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult AllocateSingleUser(AllocateUserRequest request)
        {
            _logger.LogDebug("AllocateSingleUser {userType} {application}", request.UserType, request.Application);

            lock (AllocationLock)
            {
                var user = Allocate(request);
                _logger.LogDebug("User '{username}' successfully allocated", user.Result.Username);

                var response = UserToDetailsResponseMapper.MapToResponse(user.Result);
                return Ok(response);
            }
        }

        /// <summary>
        ///     Allocate multiple users
        /// </summary>
        /// <param name="request">Allocate users request</param>
        /// <returns>Full details of an allocated users</returns>
        [HttpPatch("allocateUsers")]
        [OpenApiOperation("AllocateMultipleUsers")]
        [ProducesResponseType(typeof(List<UserDetailsResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult AllocateUsers(AllocateUsersRequest request)
        {
            _logger.LogDebug(
                "AllocateUsers No. of UserTypes: {count} Application: {application}", request.UserTypes.Count, request.Application);

            lock (AllocationLock)
            {
                var responses = new List<UserDetailsResponse>();

                foreach (var userType in request.UserTypes)
                {
                    var allocateRequest = new AllocateUserRequest()
                    {
                        AllocatedBy = request.AllocatedBy,
                        Application = request.Application,
                        ExpiryInMinutes = request.ExpiryInMinutes,
                        IsProdUser = request.IsProdUser,
                        TestType = request.TestType,
                        UserType = userType
                    };

                    var user = Allocate(allocateRequest);
                    _logger.LogDebug("User '{username}' successfully allocated", user.Result.Username);
                    responses.Add(UserToDetailsResponseMapper.MapToResponse(user.Result));
                }

                _logger.LogInformation("Allocated {count} user(s)", responses.Count);

                return Ok(responses);
            }
        }

        /// <summary>
        ///     Unallocate users by username
        /// </summary>
        /// <param name="request">List of usernames to unallocate</param>
        /// <returns>Allocation details of the unallocated users</returns>
        [HttpPatch("unallocateUsers")]
        [OpenApiOperation("UnallocateUsers")]
        [ProducesResponseType(typeof(List<AllocationDetailsResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnallocateUsersByUsername(UnallocateUsersRequest request)
        {
            _logger.LogDebug("UnallocateUsersByUsername");

            var allocations = new List<Allocation>();

            foreach (var username in request.Usernames)
            {
                var user = await GetUserByUsername(username);

                if (user == null) return NotFound();

                var allocation = await GetAllocationByUsername(user.Username);

                if (allocation == null)
                {
                    return BadRequest($"No allocation exists for user with username {user.Username}");
                }

                await Unallocate(username);

                allocation = await GetAllocationByUsername(user.Username);

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
        [OpenApiOperation("GetAllocateUsersByAllocatedBy")]
        [ProducesResponseType(typeof(List<AllocationDetailsResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllocatedUsers(string username)
        {
            _logger.LogDebug("GetAllocatedUsers {username}", username);

            var allocations = await GetAllAllocatedByUsers(username);
            _logger.LogDebug("Allocations for '{username}' successfully retrieved", username);

            var responses = allocations.Select(AllocationToDetailsResponseMapper.MapToResponse).ToList();

            _logger.LogInformation("{count} user(s) found", responses.Count);

            return Ok(responses);
        }

        private async Task<UserDto> GetUserByUsername(string username)
        {
            return await _queryHandler.Handle<GetUserByUsernameQuery, UserDto>(new GetUserByUsernameQuery(username));
        }

        private async Task<Allocation> GetAllocationByUsername(string username)
        {
            return await _queryHandler.Handle<GetAllocationByUsernameQuery, Allocation>(
                new GetAllocationByUsernameQuery(username));
        }

        private async Task<UserDto> Allocate(AllocateUserRequest request)
        {
            return await _queryHandler.Handle<GetAllocatedUserByUserTypeQuery, UserDto>(
                new GetAllocatedUserByUserTypeQuery(request));
        }

        private async Task Unallocate(string username)
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