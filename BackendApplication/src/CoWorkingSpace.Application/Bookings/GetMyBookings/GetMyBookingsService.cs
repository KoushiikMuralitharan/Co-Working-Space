using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Bookings.GetMyBookings
{
    public class GetMyBookingsService
    {
        private readonly IBookingRepository _bookingRepository;

        public GetMyBookingsService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<GetMyBookingsResult>> GetMyBookingsAsync(Guid customerId)
        {
            return await _bookingRepository.GetMyBookingsAsync(customerId);
        }
    }
}
