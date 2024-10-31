using api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//configuring the ApplicationDBContext as a service in your application, specifying that it should use SQL Server as the database provider.
//AddDbContext<ApplicationDBContext> registers ApplicationDBContext as a service in the application's dependency injection (DI) container.
//builder.Configuration.GetConnectionString("DefaultConnection") retrieves the connection string from the application’s configuration file (e.g., appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
