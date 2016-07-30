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
                PromptDialog.Text(context, sessionEntered, "Do I know you? Please enter the code you got after tour.");
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

            // stats

            // questions
            bool formCompleted;
            if (!context.ConversationData.TryGetValue("FormCompleted", out formCompleted))
            {
                var questionFormDialog = Chain.From(() => FormDialog.FromForm(FeedbackForm.BuildForm));
                context.Call(questionFormDialog, afterForm);
            }
            else
            {
                await context.PostAsync("Nice to see you again. Are you interested in anything regarding your tour?");
                await context.PostAsync("Feel free to ask tings like:\n* How tall is the building?\n* How many people work here?");
                context.Wait(SearchMessageReceived);
            }
        }

        private async Task SearchMessageReceived(IDialogContext context, IAwaitable<IMessageActivity> message)
        {
            var activity = await message;
            await context.PostAsync("Great to hear that you are interested in " + activity.Text + ". I don't have the answer yet, though.");
            context.Done(message);
        }

        private async Task afterForm(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            await context.PostAsync("Thank you for your time. To find out more about this technology visit our [GitHub repo](https://github.com/pospanet/SmartOffice).");

            context.ConversationData.SetValue("FormCompleted", true);

            var ats = new AzureTableService();
            ats.SaveAnswers(Code, await result);
            Debug.WriteLine("Answers sent to Azure.");

            context.Done(result);
        }
    }
}