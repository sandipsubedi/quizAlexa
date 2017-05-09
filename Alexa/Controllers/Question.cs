using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alexa.Controllers
{
    public class Question
    {
            public int qId { get; set; }
            public string title { get; set; }
            public string rightOption { get; set; }
            public string option1 { get; set; }
            public string option2 { get; set; }
            public string option3 { get; set; }
            public string option4 { get; set; }
    }
}