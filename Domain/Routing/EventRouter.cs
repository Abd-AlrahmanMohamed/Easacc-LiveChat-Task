namespace Domain.Routing.BaseRouter;
public partial class Router
{
    public static class EventRouter
    {
        private const string Prefix = Rule + "Event";
        public const string Get = Prefix;
        public const string GetAllEvents = Prefix + "/get-all-events";
        public const string GetEventById = Prefix + "/get-event-by-id";
        public const string AddEvent = Prefix + "/add-event";
        public const string UpdateEvent = Prefix + "/update-event";
        public const string DeleteEvent = Prefix + "/delete-event";
    }
}

