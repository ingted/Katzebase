﻿using NTDLS.ReliableMessaging;

namespace TestHarness.Payloads
{
    public class MyQuery : IRmQuery<MyQueryReply>
    {
        public string Message { get; set; }

        public MyQuery(string message)
        {
            Message = message;
        }
    }
}
