using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestApi.DAL.Commands;
using TestApi.DAL.Helpers;
using TestApi.DAL.Queries;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.UnitTests.Controllers.Allocations
{
    public class PutAllocationByUserTypeAndApplicationTests : AllocationControllerTestBase
    {
        //[Test]
        //public async Task Should_allocate_existing_available_user_by_user_type_and_application()
        //{
        //    var user = CreateUser();
        //    var allocation = new Allocation(user);
        //    var integer = new Integer(1);

        //    QueryHandlerMock
        //        .Setup(x => x.Handle<GetUserByIdQuery, User>(It.IsAny<GetUserByIdQuery>()))
        //        .ReturnsAsync(user);

        //    QueryHandlerMock
        //        .Setup(x => x.Handle<GetAllocationByUserIdQuery, Allocation>(It.IsAny<GetAllocationByUserIdQuery>()))
        //        .ReturnsAsync(allocation);

        //    QueryHandlerMock
        //        .Setup(x => x.Handle<GetNextUserNumberByUserTypeQuery, Integer>(It.IsAny<GetNextUserNumberByUserTypeQuery>()))
        //        .ReturnsAsync(integer);

        //    CommandHandlerMock.Setup(x => x.Handle(It.IsAny<AllocateByUserTypeAndApplicationCommand>()));

        //    CommandHandlerMock.Setup(x => x.Handle(It.IsAny<CreateNewAllocationByUserIdCommand>()));

        //    CommandHandlerMock.Setup(x => x.Handle(It.IsAny<CreateNewUserCommand>()));
            
        //    CommandHandlerMock.Setup(x => x.Handle(It.IsAny<AllocateByUserIdCommand>()));

        //    var response = await Controller.AllocateUserByUserTypeAndApplicationAsync(UserType.CaseAdmin, Application.TestApi);
        //    response.Should().NotBeNull();
        //    var result = (OkObjectResult)response;
        //    result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        //}
    }
}
