using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetAllocationByUserIdQuery : IQuery
    {
        public Guid UserId { get; set; }

        public GetAllocationByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }

    public class GetAllocationByUserIdQueryHandler : IQueryHandler<GetAllocationByUserIdQuery, Allocation>
    {
        private readonly TestApiDbContext _context;

        public GetAllocationByUserIdQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<Allocation> Handle(GetAllocationByUserIdQuery query)
        {
            return await _context.Allocations
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.UserId == query.UserId);
        }
    }
}
