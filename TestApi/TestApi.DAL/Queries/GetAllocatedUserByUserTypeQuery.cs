using System.Threading.Tasks;
using TestApi.Contract.Dtos;
using TestApi.Contract.Requests;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries.Core;
using TestApi.Contract.Enums;

namespace TestApi.DAL.Queries
{
    public class GetAllocatedUserByUserTypeQuery : IQuery
    {
        public GetAllocatedUserByUserTypeQuery(AllocateUserRequest request)
        {
            Application = request.Application;
            ExpiryInMinutes = request.ExpiryInMinutes;
            IsProdUser = request.IsProdUser;
            TestType = request.TestType;
            UserType = request.UserType;
            AllocatedBy = request.AllocatedBy;
        }

        public Application Application { get; set; }
        public int ExpiryInMinutes { get; set; }
        public bool IsProdUser { get; set; }
        public TestType TestType { get; set; }
        public UserType UserType { get; set; }
        public string AllocatedBy { get; set; }
    }

    public class GetAllocatedUserByUserTypeQueryHandler : IQueryHandler<GetAllocatedUserByUserTypeQuery, UserDto>
    {
        private readonly TestApiDbContext _context;
        private readonly IAllocationService _service;

        public GetAllocatedUserByUserTypeQueryHandler(TestApiDbContext context, IAllocationService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<UserDto> Handle(GetAllocatedUserByUserTypeQuery query)
        {
            var user = await _service.AllocateToService(query.UserType, query.Application,
                query.TestType, query.IsProdUser, query.ExpiryInMinutes, query.AllocatedBy);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}