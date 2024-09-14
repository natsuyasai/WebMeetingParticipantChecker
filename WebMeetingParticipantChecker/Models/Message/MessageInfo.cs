using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Message
{
    internal enum ResultCode
    {
        OK = 0,
        Close
    }

    internal class MessageInfo
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string OkButtonMessage { get; set; } = "";
        public Action<ResultCode>? OnCloseDialog { get; set; } = null;
    }

    internal class Message<T>(MessageInfo value) : ValueChangedMessage<MessageInfo>(value)
    {
    }
}
