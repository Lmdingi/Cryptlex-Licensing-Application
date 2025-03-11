using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
        void LogWarning(string message);
    }
}
