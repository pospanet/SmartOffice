using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using SmartOffice.Bot.Forms;
using System.Threading;
using SmartOffice.Bot.Services;
using System.Diagnostics;
using Microsoft.Bot.Connector;

namespace SmartOffice.Bot.Dialogs
{
    [Serializable]
    public class MainDialog : IDialog<object>
    {
        public string Code;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait<Activity>(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Activity> message)
        {
            //var activity = await message;
            //botStateClient = activity.GetStateClient();
            //botData = await botStateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);


            if (!context.ConversationData.TryGetValue("Code", out Code))
            {
                PromptDialog.Text(context, sessionEntered, "Do I know you? Please enter the code they gave you after the tour.");
            }
            else
            {
                sessionEntered(context, null);
            }
        }

        private async Task sessionEntered(IDialogContext context, IAwaitable<string> result)
        {
            if (result != null)
            {
                Code = await result;
                context.ConversationData.SetValue("Code", Code);
            }

            bool formCompleted;
            if (!context.ConversationData.TryGetValue("FormCompleted", out formCompleted))
            {
                var ats = new AzureTableService();
                var data = ats.GetTourInfo(Code);

                await context.PostAsync($"People in your group took **{data.Item2}** steps.");
                await context.PostAsync($"You contributed to that with **{data.Item1}** steps.");
                Thread.Sleep(2000);

                var questionFormDialog = Chain.From(() => FormDialog.FromForm(FeedbackForm.BuildForm, FormOptions.PromptInStart));
                context.Call(questionFormDialog, afterForm);
                //await context.Forward(questionFormDialog, afterForm, true, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Nice to see you again. Are you interested in anything regarding your tour?", null, CancellationToken.None);
                await context.PostAsync("Feel free to ask tings like:\n\r* How tall is the building?\n\r* How many people work here?");
                context.Wait(SearchMessageReceived);
            }
        }

        private async Task SearchMessageReceived(IDialogContext context, IAwaitable<IMessageActivity> message)
        {
            var activity = await message;
            await context.PostAsync("Great to hear that you are interested in " + activity.Text + ". I don't have the answer yet, though.");
            context.Wait(SearchMessageReceived);
        }

        private async Task afterForm(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            await context.PostAsync("Thank you for your time. To find out more about this technology visit our [GitHub repo](https://github.com/pospanet/SmartOffice).");

            context.ConversationData.SetValue("FormCompleted", true);

            var res = await result;

            var ats = new AzureTableService();
            ats.SaveAnswers(Code, res);
            Debug.WriteLine("Answers sent to Azure Table.");

            var abs = new AzureBlobService();
            abs.AddFeedback(res);
            Debug.WriteLine("Answers sent to Azure Blob.");

            context.Wait<Activity>(MessageReceivedAsync);
        }
    }
}