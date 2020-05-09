using System.Threading.Tasks;
using NLog;

namespace AvailabilityChecker.Notifications
{
    public class NLogAlertStrategy : IAlertStrategy
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Task Alert(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                _logger.Info(message);

            return Task.CompletedTask;
        }
    }
}