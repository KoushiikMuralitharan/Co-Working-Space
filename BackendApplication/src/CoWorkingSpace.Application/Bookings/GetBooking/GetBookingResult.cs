using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Bookings.GetBooking
{
    public class GetBookingResult
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public List<GetBookingResourceResult> Resources { get; set; } = new();

    }

    public class GetBookingResourceResult
    {
        public Guid BookableResourceId { get; set; }
        public decimal Price { get; set; }
    }

}
