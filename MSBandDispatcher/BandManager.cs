using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Microsoft.Azure.Devices.Client;
using Microsoft.Band;
using Microsoft.Band.Sensors;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    internal class BandManager
    {
        private IBandInfo Band { get; set; }

        private IBandClient Client { get; set; }
        public int LastValue { get; private set; }
        public bool ReadingLocked { get; private set; }

        private static readonly EasClientDeviceInformation ClientDeviceInformation;

        static BandManager()
        {
            ClientDeviceInformation = new EasClientDeviceInformation();
        }

        public BandManager(IBandInfo band, IBandClient client)
        {
            Band = band;
            Client = client;
            LastValue = 0;
            ReadingLocked = false;
            Client.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
        }

        private async void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            LastValue = e.SensorReading.HeartRate;
            ReadingLocked = e.SensorReading.Quality == HeartRateQuality.Locked;
            await SendDataToCloud();
        }

        private async Task SendDataToCloud()
        {
            MSBandData data = new MSBandData(LastValue, ReadingLocked, Band.Name, ClientDeviceInformation.FriendlyName);
            DeviceClient client = Microsoft.Azure.Devices.Client.DeviceClient.Create();
            Message message=new Message();
            Json.Convert
            await client.SendEventAsync(message);
        }

        public override string ToString()
        {
            return string.Concat(Band.Name, ": ", LastValue, "bpm");
        }


        public async Task StartReading()
        {
            await Client.SensorManager.HeartRate.StartReadingsAsync();
        }
    }
}
