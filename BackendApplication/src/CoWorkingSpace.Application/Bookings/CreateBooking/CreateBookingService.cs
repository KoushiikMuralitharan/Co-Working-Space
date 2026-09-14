using System;
using System.Collections.Generic;
using System.Text;
using CoWorkingSpace.Application.Common.Exceptions;
using CoWorkingSpace.Application.Common.Interfaces;

namespace CoWorkingSpace.Application.Bookings.CreateBooking
{
    public class CreateBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public CreateBookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<CreateBookingResult> CreateBookingAsync(CreateBookingRequest request)
        {
            if (request.Price < 0)
            {
                throw new BadRequestException("Booking price cannot be negative.");
            }

            var bookingId = await _bookingRepository.CreateBookingAsync(request);

            return new CreateBookingResult
            {
                BookingId = bookingId
            };
        }

    }

}
