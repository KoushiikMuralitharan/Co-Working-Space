using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Bookings.GetBooking
{
    public class GetBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<GetBookingResult?> GetBookingByIdAsync(Guid bookingId)
        {
            return await _bookingRepository.GetBookingByIdAsync(bookingId);
        }
    }
}
