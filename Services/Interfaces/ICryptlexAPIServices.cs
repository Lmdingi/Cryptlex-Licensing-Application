using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    interface ICryptlexAPIServices
    {
        Task<string> GetRequest(string endpoint);
        Task<string> CreateRequest(string data, string endpoint);
        Task<bool> DeleteRequest(string endpoint);
    }
}