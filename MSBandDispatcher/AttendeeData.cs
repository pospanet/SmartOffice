using Microsoft.WindowsAzure.Storage.Table;

namespace Pospa.NET.SmartOffice.MSBandDispatcher
{
    public class AttendeeData : TableEntity
    {
        public AttendeeData(string name, string group, long steps)
        {
            PartitionKey = group;
            RowKey = name;
            StepCount = steps;
        }

        public long StepCount { get; set; }
    }
}