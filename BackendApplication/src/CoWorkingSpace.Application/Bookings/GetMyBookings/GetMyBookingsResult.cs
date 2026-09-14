using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Bookings.GetMyBookings
{
    public class GetMyBookingsResult
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}
