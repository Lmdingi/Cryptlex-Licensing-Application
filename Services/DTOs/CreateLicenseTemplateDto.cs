using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class CreateLicenseTemplateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string FingerprintMatchingStrategy { get; set; }

        [Required]
        public bool? AllowVmActivation { get; set; }

        [Required]
        public bool? AllowContainerActivation { get; set; }

        [Required]
        public bool? UserLocked { get; set; }

        [Required]
        public bool? DisableGeoLocation { get; set; }

        [Required]
        public long Validity { get; set; }

        [Required]
        public string ExpirationStrategy { get; set; }

        [Required]
        public long AllowedActivations { get; set; }

        [Required]
        public long AllowedDeactivations { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public bool? AllowClientLeaseDuration { get; set; }

        [Required]
        public long ServerSyncGracePeriod { get; set; }

        [Required]
        public long ServerSyncInterval { get; set; }

        [Required]
        public long AllowedClockOffset { get; set; }

        [Required]
        public long ExpiringSoonEventOffset { get; set; }       
    }


}
