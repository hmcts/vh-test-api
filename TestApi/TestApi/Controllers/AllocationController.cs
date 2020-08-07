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
        /// Get the details of an allocation by user id
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>Full details of an allocation</returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(AllocationDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllocationDetailsByUserIdAsync(Guid userId)
        {
            _logger.LogDebug($"GetUserDetailsByIdAsync {userId}");

            var existingUser = await GetUserByIdAsync(userId);

            if (existingUser == null)
            {
                _logger.LogWarning($"Unable to find user with id {userId}");

                return NotFound();
            }

            var getAllocationByUserIdQuery = new GetAllocationByUserIdQuery(userId);
            var allocation = await _queryHandler.Handle<GetAllocationByUserIdQuery, Allocation>(getAllocationByUserIdQuery);

            var response = AllocationToDetailsResponseMapper.MapToResponse(allocation);
            return Ok(response);
        }

        /// <summary>
        /// Allocate user by user type and application
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
        /// Allocate user by user id
        /// </summary>
        /// <param name="userId">Type of user (e.g Judge)</param>
        /// <returns>Full details of an allocated user</returns>
        [HttpPut("{userId}")]
        [ProducesResponseType(typeof(UserDetailsResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AllocateUserByUserIdAsync(Guid userId)
        {
            _logger.LogDebug($"AllocateUserByUserIdAsync {userId}");

            var user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var allocateCommand = new AllocateByUserIdCommand(userId);
            await _commandHandler.Handle(allocateCommand);
            var allocatedUser = allocateCommand.User;
            var response = UserToDetailsResponseMapper.MapToResponse(allocatedUser);
            return Ok(response);
        }

        /// <summary>
        /// Create new allocation for new unallocated user by user id
        /// </summary>
        /// <param name="userId">User Id of the new user</param>
        /// <returns>Details of the created allocation</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserAllocationResponse), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateNewAllocationByUserIdAsync(Guid userId)
        {
            _logger.LogDebug("CreateNewAllocation");

            var user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            _logger.LogDebug($"User with id {userId} retrieved");

            var existingAllocation = await GetAllocationByUserIdAsync(userId);

            if (existingAllocation != null)
            {
                return Conflict();
            }

            var allocationId = await CreateAllocationAsync(userId);
            _logger.LogDebug($"New Allocation Created for use with id {userId}");

            var getAllocationByIdQuery = new GetAllocationByIdQuery(allocationId);
            var allocation = await _queryHandler.Handle<GetAllocationByIdQuery, Allocation>(getAllocationByIdQuery);

            var response = AllocationToDetailsResponseMapper.MapToResponse(allocation);

            _logger.LogInformation($"Created allocation for {response.Username} with id {response.Id}");

            return CreatedAtAction(nameof(CreateNewAllocationByUserIdAsync), new { allocationId = response.Id }, response);
        }

        /// <summary>
        /// Delete allocation by user id
        /// </summary>
        /// <param name="userId">User Id of the user</param>
        /// <returns>Delete an allocation</returns>
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAllocationByUserIdAsync(Guid userId)
        {
            _logger.LogDebug($"DeleteAllocationByUserIdAsync {userId}");

            var existingUser = await GetUserByIdAsync(userId);

            if (existingUser == null)
            {
                return NotFound();
            }

            _logger.LogDebug($"User with id {userId} retrieved");

            var deleteAllocationCommand = new DeleteAllocationByUserIdCommand(userId);
            
            await _commandHandler.Handle(deleteAllocationCommand);
            
            _logger.LogInformation($"Successfully deleted allocation for user with id {userId}");
            
            return NoContent();
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

        private async Task<User> GetUserByIdAsync(Guid userId)
        {
           return await _queryHandler.Handle<GetUserByIdQuery, User>(new GetUserByIdQuery(userId));
        }

        private async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _queryHandler.Handle<GetUserByUsernameQuery, User>(new GetUserByUsernameQuery(username));
        }

        private async Task<Allocation> GetAllocationByUserIdAsync(Guid userId)
        {
            return await _queryHandler.Handle<GetAllocationByUserIdQuery, Allocation>(new GetAllocationByUserIdQuery(userId));
        }

        private async Task<Allocation> GetAllocationByAllocationIdAsync(Guid allocationId)
        {
            return await _queryHandler.Handle<GetAllocationByIdQuery, Allocation>(new GetAllocationByIdQuery(allocationId));
        }

        private async Task<Guid> CreateAllocationAsync(Guid userId)
        {
            var createNewAllocationCommand = new CreateNewAllocationByUserIdCommand(userId);
            await _commandHandler.Handle(createNewAllocationCommand);
            return createNewAllocationCommand.NewAllocationId;
        }

        private async Task<User> AllocateAsync(UserType userType, Application application, int expiresInMinutes = 10)
        {
            var allocateCommand = new AllocateByUserTypeAndApplicationCommand(userType, application, expiresInMinutes);
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
