using Microsoft.Bot.Builder.FormFlow;
using SmartOffice.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.FormFlow.Json;

namespace SmartOffice.Bot.Forms
{
    [Serializable]
    public class DynamicFeedbackForm
    {
        public static IForm<DynamicFeedbackForm> BuildForm()
        {
            //var ts = new AzureTableService();
            //var questions = ts.GetQuestions();

            var form = new FormBuilder<DynamicFeedbackForm>().Message("We have some questions for you!");

            //foreach (var q in questions)
            //{
            //    form.Field(q.Id, new PromptAttribute(q.Text));
            //}

            //form.Message("Thank you for your answers.")
            //    .OnCompletion(feedbackDone);

            return form.Build();
        }

        public static IForm<JObject> BuildFormFromJson()
        {
            //var ts = new AzureTableService();
            //var questions = ts.GetQuestions();


            //var formJson = @"
            //    {
            //      ""References"": [ ],
            //      ""Imports"": [ ],
            //      ""type"": ""object"",
            //      ""required"": [],
            //      ""templates"": {},
            //      ""properties"": { }
            //    }";
            //// ""properties"": { ""1"": { ""Prompt"": { ""Patterns"": [ ""Ahoj"" ] }, ""type"": [""number"", ""null""] }}

            //JObject schema = new JObject();
            //try
            //{
            //    schema = JObject.Parse(formJson);

            //    foreach (var q in questions)
            //    {
            //        var propertiesObject = schema["properties"] as JObject;
            //        var prop = new FormProperty()
            //        {
            //            Prompt = new Prompt() { Patterns = new string[] { q.Text } },
            //            type = new string[] { "number", "null" }
            //        };

            //        propertiesObject.Add(q.Id, JToken.FromObject(prop));
            //    }


            //}
            //catch (Exception e) {
            //}


            //var form = new FormBuilderJson(schema);

            //return form.OnCompletion(feedbackJsonDone).Build();

            throw new NotImplementedException();
        }

        private static Task feedbackJsonDone(IDialogContext context, JObject state)
        {
            throw new NotImplementedException();
        }

        private static Task feedbackDone(IDialogContext context, DynamicFeedbackForm state)
        {
            throw new NotImplementedException();
        }
    }
}