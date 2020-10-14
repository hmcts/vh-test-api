using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api;
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
            HttpClient client;
            if (Zap.SetupProxy)
            {
                var handler = new HttpClientHandler
                {
                    Proxy = Zap.WebProxy,
                    UseProxy = true,
                };

                client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(Context.Config.Services.TestApiUrl)
                };
            }
            else
            {
                client = _server.CreateClient();
            }

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Context.Tokens.TestApiBearerToken}");
            return client;
        }

        protected async Task SendGetRequest(string uri)
        {
            using var client = CreateNewClient();
            Response = await client.GetAsync(uri);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendPatchRequest(string uri, string request)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            using var client = CreateNewClient();
            Response = await client.PatchAsync(uri, content);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendPostRequest(string uri, string request)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            using var client = CreateNewClient();
            Response = await client.PostAsync(uri, content);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendPutRequest(string uri, string request)
        {
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            using var client = CreateNewClient();
            Response = await client.PutAsync(uri, content);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected async Task SendDeleteRequest(string uri)
        {
            using var client = CreateNewClient();
            Response = await client.DeleteAsync(uri);
            Json = await Response.Content.ReadAsStringAsync();
        }

        protected void VerifyResponse(HttpStatusCode statusCode, bool isSuccess)
        {
            Response.StatusCode.Should().Be(statusCode);
            Response.IsSuccessStatusCode.Should().Be(isSuccess);
        }
    }
}
