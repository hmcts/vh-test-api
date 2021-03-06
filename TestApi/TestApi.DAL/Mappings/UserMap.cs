﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestApi.Domain;

namespace TestApi.DAL.Mappings
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Username).IsUnique();
            builder.HasIndex(x => x.ContactEmail).IsUnique();
            builder.Property(x => x.FirstName);
            builder.Property(x => x.LastName);
            builder.Property(x => x.DisplayName);
            builder.Property(x => x.Number);
            builder.Property(x => x.TestType);
            builder.Property(x => x.UserType);
            builder.Property(x => x.Application);
            builder.Property(x => x.IsProdUser);
            builder.Property(x => x.CreatedDate);
        }
    }
}