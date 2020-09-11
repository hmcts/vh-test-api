#nullable enable
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Queries.Core;
using TestApi.Domain;

namespace TestApi.DAL.Queries
{
    public class GetRecentUserByUsernameQuery : IQuery
    {
        public GetRecentUserByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class GetRecentUserByUsernameQueryHandler : IQueryHandler<GetRecentUserByUsernameQuery, RecentUser>
    {
        private readonly TestApiDbContext _context;

        public GetRecentUserByUsernameQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<RecentUser> Handle(GetRecentUserByUsernameQuery query)
        {
            return await _context.RecentUsers
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username.ToLower() == query.Username.ToLower());
        }
    }
}