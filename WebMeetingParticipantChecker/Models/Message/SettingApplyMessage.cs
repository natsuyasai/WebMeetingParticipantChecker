using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ZoomParticipantChecker.Model.Message
{
    internal class SettingApplyMessage : ValueChangedMessage<string>
    {
        public SettingApplyMessage(string value) : base(value)
        {
        }
    }
}
