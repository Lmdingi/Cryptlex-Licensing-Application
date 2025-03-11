using Serilog;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService()
        {
            string logPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "Logs", "log.txt");
            _logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message)
        {
            _logger.Information(message);
        }

        public void LogError(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
    }
}
