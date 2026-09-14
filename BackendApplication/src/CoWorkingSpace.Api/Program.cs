using CoWorkingSpace.Application.Bookings.CancelBookings;
using CoWorkingSpace.Application.Bookings.CreateBooking;
using CoWorkingSpace.Application.Bookings.GetBooking;
using CoWorkingSpace.Application.Bookings.GetMyBookings;
using CoWorkingSpace.Application.Common.Interfaces;
using CoWorkingSpace.Infrastructure.Persistence.Bookings;
using CoWorkingSpace.Api.Middleware;
using Scalar.AspNetCore;

    
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' was not found.");

         
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddScoped<IBookingRepository>(provider => new BookingRepository(connectionString));
builder.Services.AddScoped<CreateBookingService>();
builder.Services.AddScoped<GetBookingService>();
builder.Services.AddScoped<GetMyBookingsService>();
builder.Services.AddScoped<CancelBookingService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

