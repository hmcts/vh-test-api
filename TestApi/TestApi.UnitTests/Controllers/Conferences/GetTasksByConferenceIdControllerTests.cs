using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.Services.Clients.VideoApiClient;
using TaskStatus = TestApi.Services.Clients.VideoApiClient.TaskStatus;

namespace TestApi.UnitTests.Controllers.Conferences
{
    public class GetTasksByConferenceIdControllerTests : ConferencesControllerTestsBase
    {
        [Test]
        public async Task Should_get_tasks_by_conference_id()
        {
            var taskResponse = new List<TaskResponse>()
            {
                new TaskResponse()
                {
                    Body = "Body",
                    Created = DateTime.UtcNow,
                    Id = new long(),
                    Origin_id = Guid.NewGuid(),
                    Status = TaskStatus.ToDo,
                    Type = TaskType.Participant,
                    Updated = null,
                    Updated_by = null
                }
            };

            VideoApiClient
                .Setup(x => x.GetTasksForConferenceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(taskResponse);

            var response = await Controller.GetTasksByConferenceIdAsync(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var tasksResponse = (List<TaskResponse>)result.Value;
            tasksResponse.Should().NotBeNull();
            tasksResponse.Should().BeEquivalentTo(taskResponse);
        }

        [Test]
        public async Task Should_return_empty_list_if_no_tasks_exists()
        {
            VideoApiClient
                .Setup(x => x.GetTasksForConferenceAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TaskResponse>());

            var response = await Controller.GetTasksByConferenceIdAsync(Guid.NewGuid());
            response.Should().NotBeNull();

            var result = (OkObjectResult)response;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var tasksResponse = (List<TaskResponse>)result.Value;
            tasksResponse.Should().NotBeNull();
            tasksResponse.Count.Should().Be(0);
        }
    }
}
