using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOffice.Bot.Models
{
    public class AttendeeDataEntity : TableEntity
    {
        public AttendeeDataEntity() { }

        public AttendeeDataEntity(string name, string group, long steps)
        {
            PartitionKey = group;
            RowKey = name;
            StepCount = steps;
        }

        public long StepCount { get; set; }
    }
}