using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestApi.Contract.Responses;
using TestApi.DAL.Queries;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Services.Contracts;

namespace TestApi.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [AllowAnonymous]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IUserApiService _userApiService;

        public HealthCheckController(IQueryHandler queryHandler, IUserApiService userApiService)
        {
            _queryHandler = queryHandler;
            _userApiService = userApiService;
        }

        /// <summary>
        /// Check Service Health
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [SwaggerOperation(OperationId = "CheckServiceHealth")]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> HealthAsync()
        {
            var response = new HealthCheckResponse {Version = GetApplicationVersion()};
            try
            {
                const string username = "health";
                var query = new GetUserByUsernameQuery(username);
                await _queryHandler.Handle<GetUserByUsernameQuery, User>(query);
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
                await _userApiService.CheckHealth();
                response.UserApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.UserApiHealth.Successful = false;
                response.UserApiHealth.ErrorMessage = ex.Message;
                response.UserApiHealth.Data = ex.Data;
            }

            return response.TestApiHealth.Successful ? Ok(response) : StatusCode((int)HttpStatusCode.InternalServerError, response);
        }

        private static HealthCheckResponse.ApplicationVersion GetApplicationVersion()
        {
            var applicationVersion = new HealthCheckResponse.ApplicationVersion
            {
                Version = GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version)
            };
            return applicationVersion;
        }

        private static string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            var attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}
