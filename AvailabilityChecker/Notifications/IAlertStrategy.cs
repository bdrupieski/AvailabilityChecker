using System.Threading.Tasks;
using AvailabilityChecker.Notifications.Reactive;

namespace AvailabilityChecker.Notifications
{
    /// <summary>
    /// Alerting that works in conjunction with <see cref="SamplingAlerter{TContext}"/>.
    /// </summary>
    public interface IAlertStrategy
    {
        Task Alert(string message);
    }
}