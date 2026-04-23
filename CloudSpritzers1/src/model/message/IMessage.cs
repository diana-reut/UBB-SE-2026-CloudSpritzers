using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;

namespace CloudSpritzers1.Src.Model.Message
{
    public interface IMessage
    {
        public string GetMessage();
        public IEnumerable<FAQOption> GetNextOptions();

        public ISender GetSender();

        public int GetId();

        public Chat GetChat();

        public DateTimeOffset GetTimeStamp();
    }
}
