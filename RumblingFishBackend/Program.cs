using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Authentication;
using RumblingFishBackend.Data;
using RumblingFishBackend.Data.Seed;
using RumblingFishBackend.Models;
using RumblingFishBackend.Services;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Logs
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);
});

//Identity
builder.Services.AddScoped<IPasswordHasher<AdminAccount>, PasswordHasher<AdminAccount>>();

//Firebase
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];

FirebaseApp.Create(new AppOptions
{
    Credential = CredentialFactory
        .FromFile<ServiceAccountCredential>(firebaseCredentialsPath)
        .ToGoogleCredential()
});

//Authentication
builder.Services
    .AddAuthentication("Firebase")
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", options => { })
    .AddCookie("AdminCookie", options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
    });

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));
});

//Services custom
builder.Services.AddScoped<AdminAccountService>();
builder.Services.AddScoped<AdminPlayerService>();

builder.Services.AddHttpClient<FirebaseSaveService>();
builder.Services.AddHttpClient<GeoIpService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<LeaderboardService>();

// DB
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddControllers(); //Api only
builder.Services.AddControllersWithViews();//MVC

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

await AdminAccountSeed.SeedAsync(app);

app.Run();