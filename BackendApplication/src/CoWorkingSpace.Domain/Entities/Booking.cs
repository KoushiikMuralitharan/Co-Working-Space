using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Domain.Entities
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string Status { get; set; } = "CONFIRMED";
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "INR";
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public List<BookingResource> BookingResources { get; set; } = new();

    }
}
