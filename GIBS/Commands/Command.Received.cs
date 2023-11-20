namespace GIBS.Commands;

public partial class Command
{
    public class Received
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        readonly ILogger _logger;

        public Received(IHttpContextAccessor httpContextAccessor, ILogger<Command.Received> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        internal void Set(Command command)
            => _httpContextAccessor.HttpContext!.Items[_receivedCommandIndex] = command;

        internal Command Get()
        {
            if (_httpContextAccessor.HttpContext!.Items[_receivedCommandIndex] is Command command)
                return command;
            else
            {
                var exception = new InvalidOperationException($"{nameof(Set)} before attempting to {nameof(Get)}.");
                _logger.LogCritical(exception, "Command wasn't properly received.");
                throw exception;
            }
        }

        static readonly object _receivedCommandIndex = new();
    }
}
