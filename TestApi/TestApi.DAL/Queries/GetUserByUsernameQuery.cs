using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Mappers;
using TestApi.Contract.Dtos;
using TestApi.DAL.Queries.Core;

namespace TestApi.DAL.Queries
{
    public class GetUserByUsernameQuery : IQuery
    {
        public GetUserByUsernameQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }

    public class GetUserByUsernameQueryHandler : IQueryHandler<GetUserByUsernameQuery, UserDto>
    {
        private readonly TestApiDbContext _context;

        public GetUserByUsernameQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserByUsernameQuery query)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Username.ToLower() == query.Username.ToLower());
            return user == null ? null : UserToUserDtoMapper.Map(user);
        }
    }
}