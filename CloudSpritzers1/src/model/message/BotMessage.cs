using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;

// TODO : Maybe merge this with the regular message or pull general data in IMessage and make it abstract class instead of interface
// At this point it is not a contract of functionality but an identity
namespace CloudSpritzers1.Src.Model.Message
{
    public class BotMessage : IMessage
    {
        private int messageId;
        private ISender sender;
        private Chat chat;
        private DateTimeOffset timestamp;
        private string messageText;
        private IEnumerable<FAQOption> faqOptions;

        private BotMessage(int messageId, ISender sender, Chat chat, string messageText, IEnumerable<FAQOption> options)
        {
            this.messageId = messageId;
            this.sender = sender;
            this.chat = chat;
            this.messageText = messageText;
            this.timestamp = DateTimeOffset.UtcNow;
            this.faqOptions = options;
        }

        private BotMessage(int messageId, ISender sender, Chat chat, string messageText, IEnumerable<FAQOption> options, DateTimeOffset timestamp) : this(messageId, sender, chat, messageText, options)
        {
            this.timestamp = timestamp;
        }

        public Chat GetChat()
        {
            return this.chat;
        }

        public string GetMessage()
        {
            return this.messageText;
        }

        public ISender GetSender()
        {
            return sender;
        }

        public int GetId()
        {
            return this.messageId;
        }

        public IEnumerable<FAQOption> GetNextOptions()
        {
            return this.faqOptions;
        }

        public DateTimeOffset GetTimeStamp()
        {
            return timestamp;
        }
        public class BotMessageBuilder
        {
            private int messageId;
            private ISender sender;
            private Chat chat;
            private string messageText;
            private List<FAQOption> faqOptions;
            private DateTimeOffset timestamp;

            public BotMessageBuilder(ISender sender, Chat chat, int messageId, FAQNode nodeToMessage)
                : this(sender, chat, messageId)
            {
                this.messageText = nodeToMessage.questionText;
                this.faqOptions = nodeToMessage.options.ToList();
            }

            public BotMessageBuilder(ISender sender, Chat chat, int messageId)
            {
                this.messageText = string.Empty;
                this.messageId = messageId;
                this.sender = sender;
                this.chat = chat;
                this.faqOptions = new List<FAQOption>();
                this.timestamp = DateTimeOffset.UtcNow;
            }

            public BotMessageBuilder WithTimestamp(DateTimeOffset timestamp)
            {
                this.timestamp = timestamp;
                return this;
            }

            public BotMessageBuilder WithMessage(string setMessage)
            {
                this.messageText = setMessage;
                return this;
            }

            public BotMessageBuilder WithId(int setId)
            {
                this.messageId = setId;
                return this;
            }

            public BotMessageBuilder AddOption(FAQOption addedOption)
            {
                faqOptions.Add(addedOption);
                return this;
            }

            public BotMessageBuilder AddOptions(IEnumerable<FAQOption> setOptions)
            {
                faqOptions.Clear();
                faqOptions.AddRange(setOptions);
                return this;
            }

            public BotMessage Build()
            {
                return new BotMessage(this.messageId, this.sender, this.chat, this.messageText, this.faqOptions.ToImmutableArray(), this.timestamp);
            }
        }
    }
}
