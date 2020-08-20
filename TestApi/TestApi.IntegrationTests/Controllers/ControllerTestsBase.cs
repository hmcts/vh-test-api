using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using TestApi.IntegrationTests.Test;
using TestContext = TestApi.IntegrationTests.Test.TestContext;

namespace TestApi.IntegrationTests.Controllers
{
    public class ControllerTestsBase
    {
        protected TestContext Context;
        protected HttpResponseMessage Response;
        protected string Json;
        private TestServer _server;
        private string _bearerToken;

        [OneTimeSetUp]
        public void BeforeTestRun()
        {
            Context = new Setup().RegisterSecrets();
            _server = Context.Server;
            _bearerToken = Context.Tokens.TestApiBearerToken;
        }

        [TearDown]
        public async Task AfterEveryTest()
        {
            await Context.Data.DeleteUsers();
        }

        [OneTimeTearDown]
        public void AfterTestRun()
        {
            _server?.Dispose();
        }

        private HttpClient CreateNewClient()
        {
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearerToken}");
            return client;
        }

        protected async Task SendGetRequest(string uri)
        {
            using var client = CreateNewClient();
            Response = await client.GetAsync(new Uri(_server.BaseAddress, uri));
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendPatchRequest(string uri, string request)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            using var client = CreateNewClient();
            Response = await client.PatchAsync(new Uri(_server.BaseAddress, uri), content);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendPostRequest(string uri, string request)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            using var client = CreateNewClient();
            Response = await client.PostAsync(new Uri(_server.BaseAddress, uri), content);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendDeleteRequest(string uri)
        {
            using var client = CreateNewClient();
            Response = await client.DeleteAsync(new Uri(_server.BaseAddress, uri));
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected void VerifyResponse(HttpStatusCode statusCode, bool isSuccess)
        {
            Response.StatusCode.Should().Be(statusCode);
            Response.IsSuccessStatusCode.Should().Be(isSuccess);
        }
    }
}
