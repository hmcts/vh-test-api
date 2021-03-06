﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestApi.DAL;

namespace TestApi.Extensions
{
    public static class DatabaseMigration
    {
        public static void RunLatestMigrations(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var ctx = serviceScope.ServiceProvider.GetService<TestApiDbContext>();
            ctx.Database.Migrate();
        }
    }
}