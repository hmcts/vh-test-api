using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetAllocationByUsernameQuery : IQuery
    {
        public GetAllocationByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class GetAllocationByUsernameQueryHandler : IQueryHandler<GetAllocationByUsernameQuery, Allocation>
    {
        private readonly TestApiDbContext _context;

        public GetAllocationByUsernameQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<Allocation> Handle(GetAllocationByUsernameQuery query)
        {
            return await _context.Allocations
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username == query.Username);
        }
    }
}