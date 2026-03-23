using System.Collections.Generic;
using System.Linq;
using CloudSpritzers.src.models;

namespace CloudSpritzers.src.repository
{
    public class FAQRepository: IRepository<int, FAQEntry>
    {
        private Dictionary<int, FAQEntry> faqs = new Dictionary<int, FAQEntry>();

        public FAQEntry GetById(int id)
        {
            return faqs.ContainsKey(id) ? faqs[id] : null;
        }

        public int Add(FAQEntry elem)
        {
            faqs[elem.Id] = elem;
            return elem.Id;
        }

        public void UpdateById(int id, FAQEntry elem)
        {
            if (faqs.ContainsKey(id))
            {
                faqs[id] = elem;
            }
        }

        public void DeleteById(int id)
        {
            if (faqs.ContainsKey(id))
            {
                faqs.Remove(id);
            }

        }


        public List<FAQEntry> GetAll()
        {
            return faqs.Values.ToList();
        }

        public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
        {
            List<FAQEntry> result = new List<FAQEntry>();
            List<FAQEntry> allFaqs = faqs.Values.ToList();

            for (int i=0;i< allFaqs.Count; i++)
            {
                if (category== FAQCategoryEnum.All || allFaqs[i].Category==category)
                {
                    result.Add(allFaqs[i]);
                }
            }
            return result;

        }

        public void IncrementViewCount(int id)
        {
            var faq = GetById(id);
            if (faq != null)
                faq.ViewCount++;
        }

        public void IncrementWasHelpfulVotes(int id)
        {
            var faq = GetById(id);
            if (faq != null)
                faq.WasHelpfulVotes++;
        }

        public void IncrementWasNotHelpfulVotes(int id)
        {
            var faq = GetById(id);
            if (faq != null)
                faq.WasNotHelpfulVotes++;
        }
    }
}