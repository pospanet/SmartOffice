using System;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    public class MSBandData
    {
        public MSBandData()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public MSBandData(int heartRate, bool readingLocked, string bandName, string brokerName) : this()
        {
            HeartRate = heartRate;
            DataLocked = readingLocked;
            BandName = bandName;
            BrokerName = brokerName;
        }

        public string BandName { get; set; }
        public string BrokerName { get; set; }
        public int HeartRate { get; set; }
        public bool DataLocked { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}