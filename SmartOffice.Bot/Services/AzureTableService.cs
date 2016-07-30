using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SmartOffice.Bot.Forms;
using SmartOffice.Bot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SmartOffice.Bot.Services
{
    public class AzureTableService
    {
        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable feedbackTable;

        public AzureTableService()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            feedbackTable = tableClient.GetTableReference("feedback");
        }

        public void SaveAnswers(string code, FeedbackForm form)
        {
            FeedbackEntity fe = new FeedbackEntity()
            {
                PartitionKey = "f",
                Code = code,
                BestStation = form.BestStation.ToString(),
                BestTechnology = form.BestTechnology.ToString(),
                ConnectWithUs = form.ConnectWithUs,
                DidYouEnjoy = form.DidYouEnjoy,
                Feedback = form.Feedback,
                NotEvenTheBot = form.NotEvenTheBot,
                UseInFuture = form.UseInFuture.ToString(),
                WhyNotEnjoy = form.WhyNotEnjoy,
                WorstStation = form.WorstStation.ToString()
            };

            TableOperation insert = TableOperation.Insert(fe);
            var result = feedbackTable.Execute(insert);
        }
    }
}