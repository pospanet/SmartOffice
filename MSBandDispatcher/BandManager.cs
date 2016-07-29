using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public BandManager(IBandInfo band, IBandClient client)
        {
            Band = band;
            Client = client;
            LastValue = 0;
            ReadingLocked = false;
            Client.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
        }

        private void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            LastValue = e.SensorReading.HeartRate;
            ReadingLocked = e.SensorReading.Quality == HeartRateQuality.Locked;
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
