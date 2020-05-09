using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvailabilityChecker.Notifications
{
    public class CompositeAlertStrategy : IAlertStrategy
    {
        private readonly IEnumerable<IAlertStrategy> _alertStrategies;

        public CompositeAlertStrategy(params IAlertStrategy[] alertStrategies)
        {
            _alertStrategies = alertStrategies;
        }

        public CompositeAlertStrategy(IEnumerable<IAlertStrategy> alertStrategies)
        {
            _alertStrategies = alertStrategies;
        }

        public async Task Alert(string message)
        {
            foreach (var alertStrategy in _alertStrategies)
            {
                await alertStrategy.Alert(message);
            }
        }
    }
}