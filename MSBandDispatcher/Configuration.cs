using Microsoft.Azure.Devices.Client;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    public static class Configuration
    {
        private const string EventHubHostname = "<Put your EventHub Hostname here>";
        private const string DeviceId = "<Put your Device ID here>";
        private const string DeviceToken = "<Put your SAS key here>";

        public static class EventHub
        {
            public static string Hostname => EventHubHostname;
            public static IAuthenticationMethod Authentication => new DeviceAuthenticationWithToken(DeviceId, DeviceToken);

            public static TransportType TransportType => TransportType.Amqp;
        }
    }
}