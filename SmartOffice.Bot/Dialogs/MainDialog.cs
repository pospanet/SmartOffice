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

            PromptDialog.Text(context, sessionEntered, "Where have we met? Please enter the code you got.");
        }

        private async Task sessionEntered(IDialogContext context, IAwaitable<string> result)
        {
            Code = await result;
            //botData.SetProperty("code", code);
            
            // stats

            // questions
            var questionFormDialog = Chain.From(() => FormDialog.FromForm(FeedbackForm.BuildForm));
            context.Call(questionFormDialog, afterForm);
        }

        private async Task afterForm(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            await context.PostAsync("Thank you for your time.");

            var ats = new AzureTableService();
            ats.SaveAnswers(Code, await result);
            Debug.WriteLine("Answers sent to Azure.");

            context.Done(result);
        }
    }
}