using System;
namespace CognitiveLocator.Interfaces
{
    public interface IDeviceInstallationService
    {
        string InstallationId { get; }

        string Platform { get; }

        string PushChannel { get; }

    }
}