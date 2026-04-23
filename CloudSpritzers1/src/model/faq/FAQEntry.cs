namespace CloudSpritzers1.src.model.faq
{
    public class FAQEntry
    {
        public int Id { get; init; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public FAQCategoryEnum Category { get; set; }
        public int ViewCount { get; set; }
        public int HelpfulVotesCount { get; set; }
        public int NotHelpfulVotesCount { get; set; }

        public FAQEntry(int id, string question, string answer, FAQCategoryEnum category, int viewCount, int wasHelpfulVotes, int wasNotHelpfulVotes)
        {
            Id = id;
            Question = question;
            Answer = answer;
            Category = category;
            ViewCount = viewCount;
            HelpfulVotesCount = wasHelpfulVotes;
            NotHelpfulVotesCount = wasNotHelpfulVotes;
        }

        // These methods had 0 references. Incrementing is done directly in the database

        //public void IncrementViewCount()
        //{
        //    ViewCount++;
        //}

        //public void IncrementWasHelpfulVotes()
        //{
        //    HelpfulVotesCount++;
        //}

        //public void IncrementWasNotHelpfulVotes()
        //{
        //  NotHelpfulVotesCount++;
        //}

        public override bool Equals(object? obj)
        {
            return obj is FAQEntry entry &&
                   Id == entry.Id &&
                   Question == entry.Question &&
                   Answer == entry.Answer &&
                   Category == entry.Category &&
                   ViewCount == entry.ViewCount &&
                   HelpfulVotesCount == entry.HelpfulVotesCount &&
                   NotHelpfulVotesCount == entry.NotHelpfulVotesCount;
        }
    }
}