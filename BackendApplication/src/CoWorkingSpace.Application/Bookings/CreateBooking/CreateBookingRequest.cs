using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Bookings.CreateBooking
{
    public class CreateBookingRequest
    {
        public Guid CustomerId { get; set; }
        public Guid ResourceId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public decimal Price { get; set; }

    }
}
