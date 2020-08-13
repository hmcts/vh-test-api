using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetUserByIdQuery : IQuery
    {
        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, User>
    {
        private readonly TestApiDbContext _context;

        public GetUserByIdQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByIdQuery query)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.Id);
        }
    }
}