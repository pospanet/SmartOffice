using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Microsoft.Azure.Devices.Client;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Newtonsoft.Json;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    internal class BandManager
    {
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

        private IBandInfo Band { get; }

        private IBandClient Client { get; }
        public int LastValue { get; private set; }
        public bool ReadingLocked { get; private set; }

        private async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            LastValue = e.SensorReading.HeartRate;
            ReadingLocked = e.SensorReading.Quality == HeartRateQuality.Locked;
            await SendDataToCloud();
        }

        private async Task SendDataToCloud()
        {
            MSBandData data = new MSBandData(LastValue, ReadingLocked, Band.Name, ClientDeviceInformation.FriendlyName);
            //ToDo
            DeviceClient client = DeviceClient.Create();
            Message message =
                new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
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