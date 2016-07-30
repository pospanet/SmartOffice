using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOffice.Bot.Models
{
    public class FeedbackEntity : TableEntity
    {
        public string Code {
            get { return RowKey; }
            set { RowKey = value; }
        }

        public bool DidYouEnjoy { get; set; }

        public string WhyNotEnjoy { get; set; }

        public string BestStation { get; set; }

        public string WorstStation { get; set; }

        public string BestTechnology { get; set; }

        public bool NotEvenTheBot { get; set; }

        public string UseInFuture { get; set; }

        public bool ConnectWithUs { get; set; }

        public string Feedback { get; set; }
    }
}