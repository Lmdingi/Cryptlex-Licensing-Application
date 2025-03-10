using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class CreateLicenseKeyDto
    {
        public Guid ProductId { get; set; }
    }
}
