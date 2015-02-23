using System;

namespace Libs
{
    [Serializable]
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
    }
}
