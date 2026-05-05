using DataAccess;
using DataAccess.Repositories;
using SeawaveAPI.Middlewares;
using Services;
using Services.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException($"Connection string not found.");

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new MySqlDbConnectionFactory(connectionString));
builder.Services.Configure<SmtpAddressOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<SmtpAuthOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SessionRepository>();
builder.Services.AddScoped<MusicRepository>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<MusicService>();

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

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<SessionMiddleware>();

app.MapControllers();

app.Run();