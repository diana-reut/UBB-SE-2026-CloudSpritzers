using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Repository.Interfaces;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Interfaces;
using NSubstitute.ReceivedExtensions;

namespace CloudSpritzers1Tests.Src.MockClasses
{
    public class InMemoryFAQRepository : IFAQRepository
    {
        private List<FAQEntry> _faqEntries;

        public InMemoryFAQRepository()
        {
            _faqEntries = new List<FAQEntry>();
        }
        public int CreateNewEntity(FAQEntry elem)
        {
            foreach (var e in _faqEntries)
            {
                if (e.Equals(elem))
                    throw new ArgumentException("This FAQ already exists in the list.");
            }
            _faqEntries.Add(elem);
            return elem.Id;
        }

        public void DeleteById(int id)
        {
            var entryToDelete = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entryToDelete == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");

            _faqEntries.Remove(entryToDelete);
        }

        public IEnumerable<FAQEntry> GetAll()
        {
            return _faqEntries;
        }

        public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
        {
            if (category == FAQCategoryEnum.All)
                return _faqEntries;

            var filteredEntries = _faqEntries.Where(entry => entry.Category == category).ToList();
            return filteredEntries;
        }

        public FAQEntry GetById(int id)
        {
            var entry = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");
            return entry;
        }

        public void IncrementViewCount(int id)
        {
            var entry = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");

            entry.ViewCount = entry.ViewCount + 1;
            //var index = _faqEntries.FindIndex(e => e.Id == id);

            //var updatedEntry = new FAQEntry(entry.Id, entry.Question, entry.Answer, entry.Category, entry.ViewCount++, entry.HelpfulVotesCount, entry.NotHelpfulVotesCount);
            //_faqEntries[index] = updatedEntry;
        }

        public void IncrementWasHelpfulVotes(int id)
        {
            var entry = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");

            entry.HelpfulVotesCount = entry.HelpfulVotesCount + 1;
        }

        public void IncrementWasNotHelpfulVotes(int id)
        {
            var entry = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");

            entry.NotHelpfulVotesCount = entry.NotHelpfulVotesCount + 1;
        }

        public void UpdateById(int id, FAQEntry elem)
        {
            var entry = _faqEntries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
                throw new KeyNotFoundException($"FAQ with ID {id} not found.");
            entry.Question = elem.Question;
            entry.Answer = elem.Answer;
            entry.Category = elem.Category;
            entry.ViewCount = elem.ViewCount;
            entry.HelpfulVotesCount = elem.HelpfulVotesCount;
            entry.NotHelpfulVotesCount = elem.NotHelpfulVotesCount;


        }
    }
}
