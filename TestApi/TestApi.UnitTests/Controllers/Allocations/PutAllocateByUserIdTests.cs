using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries;
using TestApi.Domain;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class PutAllocateByUserIdTests : AllocationControllerTestBase
    {
        //[Test]
        //public async Task Should_return_OK_for_existing_user()
        //{
        //    var user = CreateUser();
        //    var allocation = new Allocation(user);

        //    QueryHandlerMock
        //        .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
        //        .ReturnsAsync(user);

        //    CommandHandlerMock
        //        .Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()))
        //        .Returns(Task.FromResult(default(object)));

        //    //CommandHandlerMock.Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()));

        //    var response = await Controller.AllocateUserByUserIdAsync(user.Id);
        //    response.Should().NotBeNull();

        //    var result = (OkObjectResult)response;
        //    result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        //}
    }
}
