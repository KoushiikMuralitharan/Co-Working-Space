using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Application.Bookings.CancelBookings
{
    public class CancelBookingResult
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
