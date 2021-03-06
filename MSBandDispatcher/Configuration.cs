﻿using Microsoft.Azure.Devices.Client;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    public static class Configuration
    {
        private const string EventHubHostname = "<EventHub Hostname>";
        private const string DeviceId = "<Device ID>";
        private const string DeviceToken = "<SAS>";
        private const string EventHubConnectionSring = "<EventHub connection string>";
        private const string StorageAccountConnectionString = "<Storage connection string>";
        public static class EventHub
        {
            public static string Hostname => EventHubHostname;
            public static IAuthenticationMethod Authentication => new DeviceAuthenticationWithToken(DeviceId, DeviceToken);

            public static TransportType TransportType => TransportType.Http1;
            public static string ConnectionString => EventHubConnectionSring;
        }

        public static class StorageAccount
        {
            public static string ConnectionString => StorageAccountConnectionString;
        }
    }
}