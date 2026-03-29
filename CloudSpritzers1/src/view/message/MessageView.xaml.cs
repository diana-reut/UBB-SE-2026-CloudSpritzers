using System;
using CloudSpritzers1.src.model.message;

namespace CloudSpritzers1.src.view.message
{
    public class MessageViewModel
    {
        private readonly IMessage _message;
        private readonly bool _isOutgoing;

        public MessageViewModel(IMessage message, bool isOutgoing)
        {
            _message = message;
            _isOutgoing = isOutgoing;
        }

        public string MessageText => _message.GetMessage();
        public string SenderName => _message.GetSender().GetName();
        public DateTimeOffset Timestamp => _message.GetTimeStamp();
        public bool IsOutgoing => _isOutgoing;
    }
}