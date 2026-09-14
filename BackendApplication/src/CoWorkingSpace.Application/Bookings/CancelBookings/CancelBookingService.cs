using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Bookings.CancelBookings
{
    public class CancelBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public CancelBookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<CancelBookingResult?> CancelBookingAsync(Guid bookingId)
        {
            var cancelled = await _bookingRepository.CancelBookingAsync(bookingId);

            if (!cancelled)
            {
                //return null;
                throw new NotFoundException("Booking not found.");
            }

            return new CancelBookingResult
            {
                BookingId = bookingId,
                Status = "CANCELLED"
            };
        }
    }
}
