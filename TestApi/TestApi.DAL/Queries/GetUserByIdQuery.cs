using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetUserByIdQuery : IQuery
    {
        public Guid Id { get; set; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
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
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.Id);

            if (user == null)
            {
                throw new UserNotFoundException(query.Id);
            }

            return user;
        }
    }
}
