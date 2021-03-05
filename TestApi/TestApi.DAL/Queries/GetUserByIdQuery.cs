using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.Common.Mappers;
using TestApi.Contract.Dtos;
using TestApi.DAL.Queries.Core;

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

    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly TestApiDbContext _context;

        public GetUserByIdQueryHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery query)
        {
            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == query.Id);

            return user == null ? null : UserToUserDtoMapper.Map(user);
        }
    }
}