using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Azure.Devices.Client;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles;
using Newtonsoft.Json;
using Pospa.NET.SmartOffice.MSBandDispatcher.Annotations;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    internal class BandManager : INotifyPropertyChanged
    {
        private static readonly Guid TileGuid;

        private static readonly EasClientDeviceInformation ClientDeviceInformation;

        static BandManager()
        {
            ClientDeviceInformation = new EasClientDeviceInformation();
            TileGuid = new Guid("A9C706CF-5A15-44C4-8215-B4052166F9D9");
        }

        public BandManager(IBandInfo band, IBandClient client)
        {
            Band = band;
            Client = client;
            LastValue = 0;
            _initialStepCount = 0;
            _lastStepCount = 0;
            ReadingLocked = false;
            Client.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
            Client.SensorManager.Pedometer.ReadingChanged += Pedometer_ReadingChanged;
        }

        private void Pedometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandPedometerReading> e)
        {
            _lastStepCount = e.SensorReading.StepsToday;
            if (_initialStepCount == 0)
            {
                _initialStepCount = _lastStepCount;
            }
        }

        private IBandInfo Band { get; }

        private IBandClient Client { get; }
        private int _lastValue;
        private int _initialStepCount;
        private int _lastStepCount;

        public int LastValue
        {
            get { return _lastValue; }
            private set
            {
                _lastValue = value;
                if (value > 0 && value < MinValue)
                {
                    MinValue = value;
                    OnPropertyChanged(nameof(MinValue));
                }
                if (value > MaxValue)
                {
                    MaxValue = value;
                    OnPropertyChanged(nameof(MaxValue));
                }
                OnPropertyChanged(nameof(LastValue));
            }
        }

        public int MinValue { get; private set; }

        public int MaxValue { get; private set; }

        public bool ReadingLocked { get; private set; }

        private async void HeartRate_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandHeartRateReading> e)
        {
            LastValue = e.SensorReading.HeartRate;
            ReadingLocked = e.SensorReading.Quality == HeartRateQuality.Locked;
            await SendDataToCloudAsync();
        }

        private async Task SendDataToCloudAsync()
        {
            MSBandData data = new MSBandData(LastValue, ReadingLocked, Band.Name, ClientDeviceInformation.FriendlyName);
            DeviceClient client = DeviceClient.CreateFromConnectionString(Configuration.EventHub.ConnectionString);
            Message message =
                new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            await client.SendEventAsync(message);
        }

        public override string ToString()
        {
            return string.Concat(Band.Name, ": ", LastValue, " bpm");
        }


        public async Task StartReadingAsync()
        {
            _initialStepCount = 0;
            await Client.SensorManager.HeartRate.StartReadingsAsync();
            await Client.SensorManager.Pedometer.StartReadingsAsync();
        }

        public async Task StopReadingAsync()
        {
            await Client.SensorManager.HeartRate.StopReadingsAsync();
            await Client.SensorManager.Pedometer.StopReadingsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task ShowIdAsync()
        {
            await Client.NotificationManager.ShowDialogAsync(TileGuid, "Guest ID", Band.Name.Split(' ').Last());
            await Client.NotificationManager.VibrateAsync(VibrationType.ThreeToneHigh);
        }

        internal static async Task UnregisterTile(IBandClient client)
        {
            await client.TileManager.RemoveTileAsync(TileGuid);
        }

        internal static async Task RegisterTile(IBandClient client)
        {
            if (client.TileManager.TileInstalledAndOwned(TileGuid, CancellationToken.None))
            {
                return;
            }
            WriteableBitmap smallIconBitmap = new WriteableBitmap(24, 24);
            BandIcon smallIcon = smallIconBitmap.ToBandIcon();
            string versionString = await client.GetHardwareVersionAsync();
            int version = int.Parse(versionString);
            WriteableBitmap tileIconBitmap = version < 10 ? new WriteableBitmap(46, 46) : new WriteableBitmap(48, 48);
            BandIcon tileIcon = tileIconBitmap.ToBandIcon();

            BandTile tile = new BandTile(TileGuid)
            {
                IsBadgingEnabled = true,
                Name = "Guided Tour",
                SmallIcon = smallIcon,
                TileIcon = tileIcon
            };

            await client.TileManager.AddTileAsync(tile);

        }
    }
}