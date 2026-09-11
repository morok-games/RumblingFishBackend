using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RumblingFishBackend.Authentication;
using RumblingFishBackend.Data;
using RumblingFishBackend.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logs
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);
});

//Firebase
var firebaseCredentialsPath = builder.Configuration["Firebase:CredentialsPath"];

FirebaseApp.Create(new AppOptions
{
    Credential = CredentialFactory
        .FromFile<ServiceAccountCredential>(firebaseCredentialsPath)
        .ToGoogleCredential()
});

builder.Services
    .AddAuthentication("Firebase")
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", options => { });

//Services custom
builder.Services.AddHttpClient<FirebaseSaveService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddHttpClient<GeoIpService>();

// DB
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
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

app.MapControllers();

app.Run();
