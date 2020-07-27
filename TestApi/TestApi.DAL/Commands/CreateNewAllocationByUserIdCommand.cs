﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestApi.DAL.Commands.Core;
using TestApi.DAL.Exceptions;
using TestApi.Domain;

namespace TestApi.DAL.Commands
{
    public class CreateNewAllocationByUserIdCommand : ICommand
    {
        public Guid UserId { get; set; }
        public Guid NewAllocationId { get; set; }

        public CreateNewAllocationByUserIdCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class CreateNewAllocationCommandHandler : ICommandHandler<CreateNewAllocationByUserIdCommand>
    {
        private readonly TestApiDbContext _context;

        public CreateNewAllocationCommandHandler(TestApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateNewAllocationByUserIdCommand command)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Id == command.UserId);

            if (user == null)
            {
                throw new UserNotFoundException(command.UserId);
            }

            var allocation = new Allocation(user);
            await _context.Allocations.AddAsync(allocation);
            await _context.SaveChangesAsync();
            command.NewAllocationId = allocation.Id;
        }
    }
}
