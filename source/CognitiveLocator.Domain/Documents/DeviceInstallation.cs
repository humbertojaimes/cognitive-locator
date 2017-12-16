using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveLocator.Domain.Documents
{
    public class DeviceInstallation
    {
        public string InstallationId { get; set; }

        public string Platform { get; set; }

        public string PushChannel { get; set; }

    }
}
