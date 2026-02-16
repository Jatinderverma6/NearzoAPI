using Microsoft.EntityFrameworkCore;
using NearzoAPI.Data;
using NearzoAPI.Services.Implementations;
using NearzoAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("CONNECTION STRING:");
Console.WriteLine(conn);
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.EnableRetryOnFailure()
    )
);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisablePreparedStatements", true);

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IDealService, DealService>();
//builder.Services.AddScoped<IOrderService, OrderService>();
//builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddControllers();
// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//
var app = builder.Build();


// =====================
// Middleware pipeline // Configure the HTTP request pipeline.
// =====================
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