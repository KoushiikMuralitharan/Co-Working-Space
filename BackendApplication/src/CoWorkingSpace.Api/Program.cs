using System.Text;

using CoWorkingSpace.Api.Middleware;

using CoWorkingSpace.Application.Authentication.Login;
using CoWorkingSpace.Application.Authentication.Register;
using CoWorkingSpace.Application.Authentication.VerifyEmail;
using CoWorkingSpace.Application.Bookings.CancelBookings;
using CoWorkingSpace.Application.Bookings.CreateBooking;
using CoWorkingSpace.Application.Bookings.GetBooking;
using CoWorkingSpace.Application.Bookings.GetMyBookings;
using CoWorkingSpace.Application.Common.Interfaces;

using CoWorkingSpace.Infrastructure.Authentication;
using CoWorkingSpace.Infrastructure.Authentication.Email;
using CoWorkingSpace.Infrastructure.Persistence.Bookings;
using CoWorkingSpace.Infrastructure.Persistence.Users;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);


// ==================================================
// Configuration
// ==================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string 'DefaultConnection' was not found.");

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is not configured.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is not configured.");

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is not configured.");


// ==================================================
// Framework Services
// ==================================================

builder.Services.AddOpenApi();

builder.Services.AddControllers();


// ==================================================
// Authentication & Authorization
// ==================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();


// ==================================================
// Infrastructure Services
// ==================================================

builder.Services.AddScoped<IUserRepository>(
    provider =>
        new UserRepository(connectionString));

builder.Services.AddScoped<IBookingRepository>(
    provider =>
        new BookingRepository(connectionString));

builder.Services.AddScoped<IEmailVerificationTokenRepository>(
    provider =>
        new EmailVerificationTokenRepository(connectionString));


// ==================================================
// Authentication Services
// ==================================================

builder.Services.AddScoped<
    IPasswordHasher,
    PasswordHasher>();

builder.Services.AddScoped<
    IVerificationTokenService,
    VerificationTokenService>();

builder.Services.AddScoped<
    IEmailService,
    EmailService>();

builder.Services.AddScoped<
    IJwtTokenService,
    JwtTokenService>();


// ==================================================
// Application Services
// ==================================================

builder.Services.AddScoped<RegisterService>();

builder.Services.AddScoped<LoginService>();

builder.Services.AddScoped<VerifyEmailService>();

builder.Services.AddScoped<CreateBookingService>();

builder.Services.AddScoped<GetBookingService>();

builder.Services.AddScoped<GetMyBookingsService>();

builder.Services.AddScoped<CancelBookingService>();


// ==================================================
// Build Application
// ==================================================

var app = builder.Build();


// ==================================================
// Middleware
// ==================================================

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();