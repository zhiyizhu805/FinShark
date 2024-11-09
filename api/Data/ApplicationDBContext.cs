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
        public DbSet<Portfolio> Portfolios { get; set; } // add new DbSet because we need to be accessing this through Entity framework
        protected override void OnModelCreating(ModelBuilder builder) //OnModelCreating(ModelBuilder builder) 方法来自于 IdentityDbContext，是 Entity Framework 用来配置模型的一个方法。builder 参数：builder 是 ModelBuilder 类型的对象，这个 ModelBuilder 由 Entity Framework 提供，并传入 OnModelCreating 方法以供你自定义数据库的配置（允许我们使用 Fluent API 配置实体的关系、属性和约束）。
        {
            base.OnModelCreating(builder); //IdentityDbContext (from which ApplicationDbContext inherits) has its own default configurations for user management (like tables for users, roles, and claims). By calling base.OnModelCreating(builder);, you keep these default configurations and settings.
            builder.Entity<Portfolio>(x=>x.HasKey(p=>new{p.AppUserId,p.StockId})); // defines a composite key for the Portfolio entity. builder.Entity<Portfolio> 表示我们正在配置 Portfolio 这个实体，例如，接下来可以定义主键、外键、表关系等。.hasKey表示定义主键。把 AppUserId 和 StockId 组合成主键

            builder.Entity<Portfolio>()
                .HasOne(u=>u.AppUser)  //HasOne 是用来定义一个“一对多”或“多对一”关系中的“一”的一方。表示每一个Portfolio关联一个AppUser
                .WithMany(u=>u.Portfolios) //WithMany 用来定义“多对一”或“多对多”关系中的“多”的一方。表示一个AppUser可以有多个Portfolio
                .HasForeignKey(p=>p.AppUserId);  //  指定 AppUserId 为 Portfolio 表中的外键，用来关联 AppUser 表的主键。

                        
            builder.Entity<Portfolio>()
                .HasOne(u=>u.Stock)  //Each Portfolio is associated with one Stock, meaning each portfolio includes a specific stock.
                .WithMany(u=>u.Portfolios)  //A Stock can be associated with multiple Portfolios, also creating a one-to-many relationship.
                .HasForeignKey(p=>p.StockId);  // StockId is a foreign key in the Portfolio table, linking to the primary key in the Stock table. This allows you to find the corresponding stock through StockId.

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