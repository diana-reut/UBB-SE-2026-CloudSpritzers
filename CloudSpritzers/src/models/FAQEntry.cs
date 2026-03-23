namespace CloudSpritzers.src.models
{
    public class FAQEntry
    {
        public int Id { get; set; }
        public string Question { get; set;}
        public string Answer { get; set; }
        public FAQCategoryEnum Category { get; set; }
        public int ViewCount { get; set; }
        public int WasHelpfulVotes { get; set; }
        public int WasNotHelpfulVotes { get; set;  }


        public FAQEntry(int id, string question, string answer, FAQCategoryEnum category)
        {
            Id = id;
            Question = question;
            Answer = answer;
            Category = category;
            ViewCount = 0;
            WasHelpfulVotes = 0;
            WasNotHelpfulVotes = 0;

        }


    }
}