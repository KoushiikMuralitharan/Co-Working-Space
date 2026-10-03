using System.IdentityModel.Tokens.Jwt;

using CoWorkingSpace.Application.Bookings.CancelBookings;
using CoWorkingSpace.Application.Bookings.CreateBooking;
using CoWorkingSpace.Application.Bookings.GetBooking;
using CoWorkingSpace.Application.Bookings.GetMyBookings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoWorkingSpace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly CreateBookingService _createBookingService;
        private readonly GetBookingService _getBookingService;
        private readonly GetMyBookingsService _getMyBookingsService;
        private readonly CancelBookingService _cancelBookingService;

        public BookingController(CreateBookingService createBookingService, GetBookingService getBookingService, GetMyBookingsService getMyBookingsService, CancelBookingService cancelBookingService)
        {
            _createBookingService = createBookingService;
            _getBookingService = getBookingService;
            _getMyBookingsService = getMyBookingsService;
            _cancelBookingService = cancelBookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingRequest request)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

            if(userIdClaim == null)
            {
                return Unauthorized();
            }

            if(!Guid.TryParse(userIdClaim.Value, out var customerId))
            {
                return Unauthorized();
            }

            request.CustomerId = customerId;

            var result = await _createBookingService.CreateBookingAsync(request);

            return Ok(result);
            
        }

        [HttpGet("{bookingId:guid}")]
        public async Task<IActionResult> GetBooking(Guid bookingId)
        {
            var result = await _getBookingService.GetBookingByIdAsync(bookingId);

            if(result == null)
            {
                return NotFound(new 
                {
                    message = "Booking not found."
                });

            }

            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings(Guid customerId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub);

            if(userIdClaim == null)
            {
                return Unauthorized();
            }

            if(!Guid.TryParse(userIdClaim.Value, out var custommerId))
            {
                return Unauthorized();
            }

            var result =
                await _getMyBookingsService.GetMyBookingsAsync(customerId);

            return Ok(result);
        }

        [HttpPost("{bookingId:guid}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var result = await _cancelBookingService.CancelBookingAsync(bookingId);

            return Ok(result);
        }
    }
}
