using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Captivate.Common.Partners.Push
{
    public class PushcrewModel
    {
        public class ValidKeyResponse
        {
            public string status { get; set; }
            public string message { get; set; }
        }
        public class MessageRequest
        {
            public string title { get; set; }
            public string message { get; set; }
            public string url { get; set; }
            public string image_url { get; set; }
            public string hero_image_url { get; set; }
            public string button_one_label { get; set; }
            public string button_one_url { get; set; }
            public string button_two_label { get; set; }
            public string button_two_url { get; set; }
            public int time_to_live { get; set; }
            public int autohide_notification { get; set; }
        }

        public class UTM
        {
            public string source { get; set; }
            public string medium { get; set; }
            public string campaign { get; set; }
        }

        public class METADATA
        {
            public string additionalProp1 { get; set; }
            public string additionalProp2 { get; set; }
            public string additionalProp3 { get; set; }
        }

        public class MessageResponse
        {
            public string uuid { get; set; }
        }

    }
}
