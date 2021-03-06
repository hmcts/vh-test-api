using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BookingsApi.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using TestApi.Contract.Dtos;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using UserApi.Client;
using VideoApi.Client;

namespace TestApi.Controllers
{
    [Produces("application/json")]
    [Route("health")]
    [AllowAnonymous]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IBookingsApiClient _bookingsApiClient;
        private readonly IQueryHandler _queryHandler;
        private readonly IUserApiClient _userApiClient;
        private readonly IVideoApiClient _videoApiClient;

        public HealthController(IQueryHandler queryHandler, IBookingsApiClient bookingsApiClient,
            IUserApiClient userApiClient, IVideoApiClient videoApiClient)
        {
            _queryHandler = queryHandler;
            _bookingsApiClient = bookingsApiClient;
            _userApiClient = userApiClient;
            _videoApiClient = videoApiClient;
        }

        /// <summary>
        ///     Check Service Health
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [OpenApiOperation("CheckServiceHealth")]
        [ProducesResponseType(typeof(HealthResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HealthResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> HealthAsync()
        {
            var response = new HealthResponse {Version = GetApplicationVersion()};
            try
            {
                const string username = "health";
                var query = new GetUserByUsernameQuery(username);
                await _queryHandler.Handle<GetUserByUsernameQuery, UserDto>(query);
                response.TestApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.TestApiHealth.Successful = false;
                response.TestApiHealth.ErrorMessage = ex.Message;
                response.TestApiHealth.Data = ex.Data;
            }

            try
            {
                await _bookingsApiClient.CheckServiceHealthAsync();
                response.BookingsApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.BookingsApiHealth.Successful = false;
                response.BookingsApiHealth.ErrorMessage = ex.Message;
                response.BookingsApiHealth.Data = ex.Data;
            }

            try
            {
                await _userApiClient.CheckServiceHealthAsync();
                response.UserApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.UserApiHealth.Successful = false;
                response.UserApiHealth.ErrorMessage = ex.Message;
                response.UserApiHealth.Data = ex.Data;
            }

            try
            {
                await _videoApiClient.CheckServiceHealthAsync();
                response.VideoApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.VideoApiHealth.Successful = false;
                response.VideoApiHealth.ErrorMessage = ex.Message;
                response.VideoApiHealth.Data = ex.Data;
            }

            return response.TestApiHealth.Successful && response.BookingsApiHealth.Successful &&
                   response.UserApiHealth.Successful && response.VideoApiHealth.Successful
                ? Ok(response)
                : StatusCode((int) HttpStatusCode.InternalServerError, response);
        }

        private static AppVersionResponse GetApplicationVersion()
        {
            var applicationVersion = new AppVersionResponse()
            {
                FileVersion = GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version),
                InformationVersion = GetExecutingAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion)
            };
            return applicationVersion;
        }

        private static string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            var attribute = (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}