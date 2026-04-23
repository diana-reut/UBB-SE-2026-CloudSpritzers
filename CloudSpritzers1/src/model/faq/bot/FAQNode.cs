using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Service.Bot;

namespace CloudSpritzers1.Src.Model.Faq.Bot
{
    public record FAQNode(
        int faqNodeId,
        string questionText,
        ImmutableArray<FAQOption> options,
        bool isFinalAnswer);
}
