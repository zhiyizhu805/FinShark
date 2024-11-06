using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                },
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
/*
OnModelCreating: A method provided by IdentityDbContext to configure the database schema. 
Overriding it here allows for custom configurations.

ModelBuilder: Used to define entity properties and relationships in the database.

base.OnModelCreating(builder): Calls the base OnModelCreating method to retain default 
IdentityDbContext configurations (e.g., for users and roles).

NormalizedName: A standardized version of IdentityRole names, ensuring case-insensitive role matching (e.g., "Admin" becomes "ADMIN").

HasData: Seeds the database with two default roles (Admin and User) during initialization.


ApplicationDBContext: A database context class that inherits from DbContext, enabling database interactions.
DbSet<Stock> and DbSet<Comment>: Represent the Stock and Comment tables in the database, allowing CRUD operations on these tables.
Usage: By using ApplicationDBContext, Entity Framework Core identifies and manages tables, providing a way to perform database operations without writing SQL.
*/