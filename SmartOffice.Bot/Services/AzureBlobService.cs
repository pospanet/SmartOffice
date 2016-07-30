using Microsoft.IdentityModel.Protocols;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SmartOffice.Bot.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SmartOffice.Bot.Services
{
    public class AzureBlobService
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer feedbackContainer;

        public AzureBlobService()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            blobClient = storageAccount.CreateCloudBlobClient();
            feedbackContainer = blobClient.GetContainerReference("feedback");
        }

        public void AddFeedback(FeedbackForm filledForm)
        {
            CloudAppendBlob appBlob = feedbackContainer.GetAppendBlobReference("feedbacks.csv");
            if (!appBlob.Exists())
            {
                appBlob.CreateOrReplace();
            }

            appBlob.AppendText($"{filledForm.DidYouEnjoy},{filledForm.WhyNotEnjoy},{filledForm.BestStation.ToString()},{filledForm.WorstStation.ToString()},{filledForm.BestTechnology},{filledForm.NotEvenTheBot},{filledForm.UseInFuture},{filledForm.ConnectWithUs},{filledForm.Feedback}");

            Debug.WriteLine(appBlob.DownloadText());
        }

    }
}