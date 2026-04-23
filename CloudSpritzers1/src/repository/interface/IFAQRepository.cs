using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.model.faq;

namespace CloudSpritzers1.src.repository.interfaces
{
    public interface IFAQRepository : IRepository<int, FAQEntry>
    {
        List<FAQEntry> GetByCategory(FAQCategoryEnum category);
        void IncrementViewCount(int id);
        void IncrementWasHelpfulVotes(int id);
        void IncrementWasNotHelpfulVotes(int id);
    }
}
