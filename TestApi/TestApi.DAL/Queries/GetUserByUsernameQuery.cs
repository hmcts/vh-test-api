#nullable enable
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Exceptions;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetUserByUsernameQuery : IQuery
    {
        public string Username { get; set; }

        public GetUserByUsernameQuery(string username)
        {
            Username = username;
        }
    }

    public class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, User>
    {
        private readonly TestApiDbContext _context;

        public GetUserByUsernameQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(GetUserByUsernameQuery query)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username.ToLower() == query.Username.ToLower());
        }
    }
}
