namespace Domain.Routing.BaseRouter;
public partial class Router
{
    public static class BookingRouter
    {
        private const string Prefix = Rule + "Booking";
        public const string Get = Prefix;
        public const string GetAllBookings = Prefix + "/get-all-bookings";
        public const string GetBookingtById = Prefix + "/get-booking-by-id";
        public const string AddBooking = Prefix + "/add-booking";
        public const string DeleteBooking = Prefix + "/delete-booking";
    }
}