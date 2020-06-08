using Microsoft.Extensions.Logging;

namespace ConsoleApp.Example
{
    public class Application
    {
        private readonly ILogger<Application> _logger;

        public Application(ILogger<Application> logger)
        {
            _logger = logger;
        }

        public void Run()
        {
            _logger.LogInformation("HuHu :)");
        }
    }
}
