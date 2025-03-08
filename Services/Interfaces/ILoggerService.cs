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
        void LogError(string message, Exception ex);
        void LogWarning(string message);
    }
}
