using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NearzoAPI.Data;
using NearzoAPI.Entities;
using NearzoAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.CommandTimeout(60);
        }
    )
);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisablePreparedStatements", true);

// Register services
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IDealService, DealService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddControllers();
// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//
//

var app = builder.Build();


// =====================
// Middleware pipeline // Configure the HTTP request pipeline.
// =====================
    app.UseSwagger();
    app.UseSwaggerUI();

// In local → HTTPS             ✔ In Render → No redirect loop
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
//app.Run();