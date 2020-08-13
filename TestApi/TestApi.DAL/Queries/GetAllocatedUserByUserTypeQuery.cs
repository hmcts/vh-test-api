using System.Threading.Tasks;
using TestApi.DAL.Commands;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.Queries
{
    public class GetAllocatedUserByUserTypeQuery : IQuery
    {
        public GetAllocatedUserByUserTypeQuery(UserType userType, Application application,
            int extendedExpiryInMinutes = 10)
        {
            UserType = userType;
            Application = application;
            ExtendedExpiryInMinutes = extendedExpiryInMinutes;
        }

        public UserType UserType { get; set; }
        public Application Application { get; set; }
        public int ExtendedExpiryInMinutes { get; set; }
    }

    public class GetNewAllocationByUserTypeQueryHandler : IQueryHandler<GetAllocatedUserByUserTypeQuery, User>
    {
        private readonly TestApiDbContext _context;
        private readonly IAllocationService _service;

        public GetNewAllocationByUserTypeQueryHandler(TestApiDbContext context, IAllocationService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<User> Handle(GetAllocatedUserByUserTypeQuery query)
        {
            var user = await _service.AllocateToService(query.UserType, query.Application,
                query.ExtendedExpiryInMinutes);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}