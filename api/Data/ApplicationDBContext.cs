using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace api.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        :base(dbContextOptions)
        {
            
        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}

/*
ApplicationDBContext: A database context class that inherits from DbContext, enabling database interactions.
DbSet<Stock> and DbSet<Comment>: Represent the Stock and Comment tables in the database, allowing CRUD operations on these tables.
Usage: By using ApplicationDBContext, Entity Framework Core identifies and manages tables, providing a way to perform database operations without writing SQL.
*/