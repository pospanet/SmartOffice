using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace SmartOffice.Bot.Forms
{
    public enum Stations
    {
        [Describe("Azure Wall", "https://czsmartoffice.blob.core.windows.net/images/MS.AzureWall.foto.jpg")]
        AzureWall = 1,
        [Describe("Living Room", "https://czsmartoffice.blob.core.windows.net/images/MS.Livingroom.foto.jpg")]
        LivingRoom,
        GalleryWall,
        Hubs,
        OfficeTour,
        All,
        None
    }

    public enum Technologies
    {
        Azure = 1, PowerBI, CortanaIntelligenceSuite, Office365, SkypeForBusiness, TheBot, Nothing
    }

    [Serializable]
    [Template(TemplateUsage.EnumSelectOne, "{&} {||}", FieldCase=CaseNormalization.None)]
    public class FeedbackForm
    {
        [Prompt("Did you enjoy your visit in our office? {||}")]
        public bool DidYouEnjoy;

        [Prompt("Oh, why not?")]
        public string WhyNotEnjoy;

        [Describe("Which station did you like the most?")]
        public Stations BestStation;

        [Describe("And which station was the worst?")]
        public Stations WorstStation;

        [Describe("What about technologies? Were any of them interesting to you?")]
        public Technologies BestTechnology;

        [Prompt("Really nothing? Do you at least like me - the bot? {||}")]
        public bool NotEvenTheBot;

        [Describe("Back to technologies. Do you think you could use any of those in future?")]
        public Technologies UseInFuture;

        [Prompt("Do you want us to connect you with our professionals within these areas? {||}")]
        public bool ConnectWithUs;

        [Prompt("Is there anything else you would like to send as a feedback? I'll forward what you type to the team.")]
        public string Feedback;

        public static IForm<FeedbackForm> BuildForm()
        {
            return new FormBuilder<FeedbackForm>()
                .Field(nameof(DidYouEnjoy))
                .Message("I'm glad to hear that!", (state) => state.DidYouEnjoy == true)
                .Field(nameof(WhyNotEnjoy), (state) => state.DidYouEnjoy == false)
                .Message("Feedback taken, we will try and make the experience better.", (state) =>  state.DidYouEnjoy == false && state.WhyNotEnjoy != string.Empty)
                .Field(nameof(BestStation))
                .Message("Amazing!", (state) => state.BestStation == Stations.All)
                .Field(nameof(WorstStation))
                .Message("Wow, that's bad. I'll let the team know to improve.", (state) => state.WorstStation == Stations.All)
                .Field(nameof(BestTechnology))
                .Field(nameof(NotEvenTheBot), (state) => state.BestTechnology == Technologies.Nothing, async (state, value) => {
                    if ((bool)value == true)
                    {
                        state.BestTechnology = Technologies.TheBot;
                    }

                    return new ValidateResult() { IsValid = true, Value = value };
                })
                .Message("At least something. Good.", (state) => state.NotEvenTheBot == true)
                .Field(nameof(UseInFuture))
                .Field(nameof(ConnectWithUs))
                .Field(nameof(Feedback))
                .OnCompletion(formComplete)
                .Build();
        }

        private async static Task formComplete(IDialogContext context, FeedbackForm state)
        {
            context.Done(state);
        }
    }
}
