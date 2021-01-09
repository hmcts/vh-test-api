using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetAllAllocationsForAUserQuery : IQuery
    {
        public GetAllAllocationsForAUserQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class GetAllAllocationsForAUserQueryHandler : IQueryHandler<GetAllAllocationsForAUserQuery, List<Allocation>>
    {
        private readonly TestApiDbContext _context;

        public GetAllAllocationsForAUserQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Allocation>> Handle(GetAllAllocationsForAUserQuery query)
        {
            return await _context.Allocations
                .Where(x =>
                    x.AllocatedBy.ToLower() == query.Username.ToLower() &&
                    x.IsAllocated())
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
