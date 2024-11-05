using api.Data;
using api.Interfaces;
using api.Models;
using api.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//configuring the ApplicationDBContext as a service in your application, specifying that it should use SQL Server as the database provider.
//AddDbContext<ApplicationDBContext> registers ApplicationDBContext as a service in the application's dependency injection (DI) container.
//builder.Configuration.GetConnectionString("DefaultConnection") retrieves the connection string from the application’s configuration file (e.g., appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
)
    };
});


builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();



/*

1. Understanding Include and Entity Relationships
csharp
Copy code
return _context.Stocks.Include(s => s.Comments).ToListAsync();
The Include method manages entity relationships by eagerly loading related data. Here, s => s.Comments tells .NET to load Comments along with each Stock. This is particularly helpful for one-to-many relationships (like Stock to Comments) to avoid additional database queries when accessing related data, enhancing performance.

s => s.Comments means s is an instance of Stock, and .Comments is the list of related Comment entities.
2. Avoiding Object Cycles: Why ReferenceLoopHandling.Ignore Is Needed
csharp
Copy code
builder.Services.AddControllers().AddNewtonsoftJson(options => 
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
Object cycles (or circular references) occur when entities reference each other, such as Stock -> Comments -> Stock. This setup can cause infinite loops during JSON serialization. By setting ReferenceLoopHandling.Ignore, the JSON serializer ignores these circular references, preventing infinite loops and serialization errors.

3. Errors I made
Error 1: Using CommentRepository instead of ICommentRepository for dependency injection. Using interfaces ensures proper dependency resolution and keeps the controller loosely coupled.
Error 2: Mismatched method signatures between the interface and implementation caused method resolution issues. Ensure that method names and signatures match in both interface and class implementations.


**********************************************************



*/