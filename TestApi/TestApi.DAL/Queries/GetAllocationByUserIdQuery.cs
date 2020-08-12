using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Exceptions;
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
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == query.UserId);

            if (user == null)
            {
                throw new UserNotFoundException(query.UserId);
            }

            return await _context.Allocations.SingleOrDefaultAsync(x => x.User.Id == user.Id);
        }
    }
}
