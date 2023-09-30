using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Models.Message
{
    internal class MessageInfo
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
    }

    internal class Message<T> : ValueChangedMessage<MessageInfo>
    {
        public Message(MessageInfo value) : base(value)
        {
        }
    }
}
