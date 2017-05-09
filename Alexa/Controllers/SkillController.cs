using System.Web.Http;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Alexa.Controllers
{
    public class SkillController : ApiController
    {
        string rightOption;
        int questionId;

        [HttpPost]
        public dynamic Post(AlexaRequest request)
        {
            AlexaResponse response;
            switch (request.Request.Intent.Name)
            {
                case "HelloWorldIntent":
                    response = InvokeHelloWorldIntent(request);
                    break;

                case "AskQuestionOverSomeTopicIntent":
                    response = InvokeAskQuestionOverSomeTopicIntent(request);
                    break;

                case "OptionsIntent":
                    response = InvokeOptionsIntent(request);
                    break;

                case "YesNoIntent":
                    response = InvokeYesNoIntent(request);
                    break;

                case "AMAZON.StopIntent":
                    response = InvokeAmazonStopIntent(request);
                    break;

                case "AMAZON.HelpIntent":
                    response = InvokeAmazonHelpIntent(request);
                    break;

                default:
                    response = InvokeDefaultIntent(request);
                    break;
            }
            return response;
        }

        private AlexaResponse InvokeAmazonHelpIntent(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse();
            response.Response.ShouldEndSession = false;
            response.Response.OutputSpeech.Text = "I am very happy to help you today. You can say Alexa, ask quiz collections a question over Java or any other topic to get quiz questions.";

            return response;

        }

        private AlexaResponse InvokeAmazonStopIntent(AlexaRequest request)
        {
            AlexaResponse response = new AlexaResponse();
            response.Response.ShouldEndSession = true;
            response.Response.OutputSpeech.Text = "Thanks for Visiting quiz collections. Have a great day";

            return response;
        }

        private AlexaResponse InvokeYesNoIntent(AlexaRequest request)
        {
            var tempResponse = new AlexaResponse();
            var slots = request.Request.Intent.GetSlots();

            String key = "";
            String value = "";

            if (slots.Count < 1)
            {
                tempResponse = new AlexaResponse("Sorry, I couldn't understand that");
            }
            else
            {

                for (var i = 0; i < slots.Count; i++)
                {
                    key = slots[i].Key;
                    value = slots[i].Value;
                }

            }

            tempResponse.Response.ShouldEndSession = false;
            tempResponse.Response.OutputSpeech.Type = "SSML";
            tempResponse.Response.OutputSpeech.Ssml = "<speak>" + "The slots were : " + key + "value :" + value + "</speak>";

            return tempResponse;
        }

        private AlexaResponse InvokeDefaultIntent(AlexaRequest request)
        {
            // When user utters something like : alexa start quiz collections
            AlexaResponse response = new AlexaResponse();
            response.Response.ShouldEndSession = false;
            response.Response.OutputSpeech.Text = "Welcome to Quiz Collections. You can start by saying , ask me a question over Java or any other topic that you like.";
            return response;
        }

        private AlexaResponse InvokeOptionsIntent(AlexaRequest request)
        {
            var response = new AlexaResponse();
            var slots = request.Request.Intent.GetSlots();

            String key = "";
            String valueFromSlots = "";

            if (slots.Count < 1)
            {
                // tempResponse = new AlexaResponse("Sorry, I couldn't understand that");
            }
            else
            {
                for (var i = 0; i < slots.Count; i++)
                {
                    key = slots[i].Key;
                    valueFromSlots = slots[i].Value;
                }

            }
            response.Response.ShouldEndSession = false;
            response.Response.OutputSpeech.Type = "SSML";

            string converted = "";

            if(valueFromSlots=="A" || valueFromSlots == "a")
            {
                converted = "first";
            }
            else if (valueFromSlots == "B" || valueFromSlots == "b")
            {
                converted = "second";
            }
            else if (valueFromSlots == "C" || valueFromSlots == "c")
            {
                converted = "third";
            }
            else if (valueFromSlots == "D" || valueFromSlots == "d")
            {
                converted = "fourth";
            }


            // Check if answer was right
            if (converted == request.Session.Attributes.RightOption)
            {
                response.Response.OutputSpeech.Ssml = "Great Job !";
            }
            else
            {
                response.Response.OutputSpeech.Ssml = "Sorry that was not the right answer. You can try again by saying, <s> Option A </s> or <s> Option B </s> or <s> Option C </s> or <s> Option D. </s>";
            }

            response.Session.RightOption = request.Session.Attributes.RightOption;

            return response;

        }

        private AlexaResponse InvokeAskQuestionOverSomeTopicIntent(AlexaRequest request)
        {
            AlexaResponse tempResponse = new AlexaResponse();

            String key = "";
            String value = "";

            var slots = request.Request.Intent.GetSlots();

            if (slots.Count < 1)
            {
                tempResponse = new AlexaResponse("Sorry, I couldn't understand that");
            }
            else
            {
                for (var x = 0; x < slots.Count; x++)
                {
                    key = slots[x].Key;
                    value = slots[x].Value;
                }

                tempResponse.Response.ShouldEndSession = false;
                tempResponse.Response.OutputSpeech.Type = "SSML";
                // value is the topic that was requested
                tempResponse.Response.OutputSpeech.Ssml = "<speak>" + requestQuestion(value) + "</speak>";
            }

            tempResponse.Session.QuestionId = questionId;
            tempResponse.Session.RightOption = rightOption;

            return tempResponse;
        }

        private AlexaResponse InvokeHelloWorldIntent(AlexaRequest request)
        {
            return new AlexaResponse("Hello, From QuizCollections dot com");
        }

        public String requestQuestion(String keyword)
        {

            // request the question in JSON format from here ;
            // http://stackoverflow.com/questions/5566942/how-to-get-a-json-string-from-url

            string title;
            string option1;
            string option2;
            string option3;
            string option4;

            // make a https request from here ::
            String topic = keyword.Trim();

            using (WebClient wc = new WebClient())
            {
                var mainURL = "https://quizcollections.com/api/question/" + topic.ToLower();
                var json = wc.DownloadString(mainURL);

                System.Diagnostics.Debug.Print("json was: ------");
                System.Diagnostics.Debug.Print(json);

                Question q = JsonConvert.DeserializeObject<Question>(json);

                title = q.title;
                option1 = q.option1;
                option2 = q.option2;
                option3 = q.option3;
                option4 = q.option4;
                rightOption = q.rightOption;
                questionId = q.qId;   
            }


            // Make database call...

            var question = "<s> " + title + " </s>";
            var optionA = "<s> " + " <break strength='strong'/> " + "Option<s> A</s> " + option1 + " </s>";
            var optionB = "<s> " + " <break strength='strong'/> " + "Option B " + option2 + " </s>";
            var optionC = "<s> " + " <break strength='strong'/> " + "Option C " + option3 + " </s>";
            var optionD = "<s> " + " <break strength='strong'/>" + "Option D " + option4 + " </s>";
            return question + optionA + optionB + optionC + optionD;


        }
    }
}


