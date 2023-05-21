using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ZoomParticipantChecker.Model.Message
{
    internal class GettingScheduleErrorMessage : ValueChangedMessage<string>
    {
        public GettingScheduleErrorMessage(string value) : base(value)
        {
        }
    }
}
