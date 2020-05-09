using System.Threading.Tasks;

namespace AvailabilityChecker.Notifications.Slack
{
    public class SlackAlertStrategy : IAlertStrategy
    {
        private readonly SlackWebHook _slackWebHook;

        public SlackAlertStrategy(string slackWebHookUrl)
        {
            _slackWebHook = new SlackWebHook(slackWebHookUrl);
        }

        public SlackAlertStrategy(SlackWebHook slackWebHook)
        {
            _slackWebHook = slackWebHook;
        }

        public async Task Alert(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                await _slackWebHook.PostMessage(message);
            }
        }
    }
}