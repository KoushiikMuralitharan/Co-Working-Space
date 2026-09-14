using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Bookings.CreateBooking;
using CoWorkingSpace.Application.Bookings.GetBooking;
using CoWorkingSpace.Application.Bookings.GetMyBookings;

namespace CoWorkingSpace.Application.Common.Interfaces
{
    public interface IBookingRepository
    {
        Task<Guid> CreateBookingAsync(CreateBookingRequest request);
        Task<GetBookingResult?> GetBookingByIdAsync(Guid bookingId);
        Task<List<GetMyBookingsResult>> GetMyBookingsAsync(Guid customerId);
        Task<bool> CancelBookingAsync(Guid bookingId);

    }
}
