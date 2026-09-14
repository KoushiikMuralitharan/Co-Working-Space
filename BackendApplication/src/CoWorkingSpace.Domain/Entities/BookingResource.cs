using System;
using System.Collections.Generic;
using System.Text;

namespace CoWorkingSpace.Domain.Entities
{
    public class BookingResource
    {
        public Guid BookingResourceId { get; set; }
        public Guid BookingId { get; set; }
        public Guid BookableResourceId { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

    }
}
