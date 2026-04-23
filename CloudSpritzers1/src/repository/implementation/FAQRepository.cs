using System;
using System.Collections.Generic;
using System.Linq;
using CloudSpritzers1.Src.Model.Faq;
using Microsoft.Data.SqlClient;
using CloudSpritzers1.Src.Repository.Database;
using CloudSpritzers1.Src.Repository.Interfaces;

namespace CloudSpritzers1.Src.Repository.Implementation
{
  public class FAQRepository : DatabaseRepository<int, FAQEntry>, IFAQRepository
    {
    public FAQRepository()
        {
        }

    public FAQEntry GetById(int faqId)
        {
            SqlCommand command = new SqlCommand("SELECT * FROM FAQEntry WHERE FAQentry_id = @id");
            command.Parameters.AddWithValue("@id", faqId);

            FAQEntry faq = GetById(faqId, command);

            if (faq == null)
            {
                throw new KeyNotFoundException($"FAQ with id {faqId} was not found.");
            }

            return faq;
        }

    public int CreateNewEntity(FAQEntry elem)
    {
            if (elem == null)
            {
                throw new ArgumentNullException(nameof(elem), "FAQ entry cannot be null.");
            }

            SqlCommand command = new SqlCommand(
                "INSERT INTO FAQEntry (question, answer, category) " +
                "OUTPUT INSERTED.FAQentry_id " +
                "VALUES (@question, @answer, @category)");

            command.Parameters.AddWithValue("@question", elem.Question);
            command.Parameters.AddWithValue("@answer", elem.Answer);
            command.Parameters.AddWithValue("@category", elem.Category.ToString());

            int addedEntityId = Add(command, elem);
            InvalidateCacheEntry(addedEntityId);
            return addedEntityId;
        }

    public void UpdateById(int id, FAQEntry elem)
    {
            if (elem == null)
            {
                throw new ArgumentNullException(nameof(elem), "FAQ entry cannot be null.");
            }

        SqlCommand command = new SqlCommand(
            "UPDATE FAQEntry " +
            "SET question = @question, " +
            "answer = @answer, " +
            "category = @category, " +
            "view_count = @viewCount, " +
            "was_helpful_votes = @wasHelpfulVotes, " +
            "was_not_helpful_votes = @wasNotHelpfulVotes " +
            "WHERE FAQentry_id = @id");

        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@question", elem.Question);
        command.Parameters.AddWithValue("@answer", elem.Answer);
        command.Parameters.AddWithValue("@category", elem.Category.ToString());
        command.Parameters.AddWithValue("@viewCount", elem.ViewCount);
        command.Parameters.AddWithValue("@wasHelpfulVotes", elem.HelpfulVotesCount);
        command.Parameters.AddWithValue("@wasNotHelpfulVotes", elem.NotHelpfulVotesCount);

        UpdateById(id, command, elem);
        InvalidateCacheEntry(id);
    }

    public void DeleteById(int id)
    {
            SqlCommand command = new SqlCommand("DELETE FROM FAQEntry WHERE FAQentry_id = @id");
            command.Parameters.AddWithValue("@id", id);

            DeleteById(id, command);
    }

    public IEnumerable<FAQEntry> GetAll()
    {
        SqlCommand command = new SqlCommand("SELECT * FROM FAQEntry");
        return GetAll(command);
    }

    public List<FAQEntry> GetByCategory(FAQCategoryEnum category)
    {
            SqlCommand command;

            if (category == FAQCategoryEnum.All)
            {
                command = new SqlCommand("SELECT * FROM FAQEntry");
            }
            else
            {
                command = new SqlCommand("SELECT * FROM FAQEntry WHERE category = @category");
                command.Parameters.AddWithValue("@category", category.ToString());
            }

            return GetAll(command).ToList();
    }

    public void IncrementViewCount(int id)
    {
            SqlCommand command = new SqlCommand(
                "UPDATE FAQEntry SET view_count = view_count + 1 WHERE FAQentry_id = @id");
            command.Parameters.AddWithValue("@id", id);

            ExecuteNonQuery(command);
            InvalidateCacheEntry(id);
        }

    public void IncrementWasHelpfulVotes(int id)
    {
            SqlCommand command = new SqlCommand(
                "UPDATE FAQEntry SET was_helpful_votes = was_helpful_votes + 1 WHERE FAQentry_id = @id");
            command.Parameters.AddWithValue("@id", id);

            ExecuteNonQuery(command);
            InvalidateCacheEntry(id);
        }

    public void IncrementWasNotHelpfulVotes(int id)
    {
            SqlCommand command = new SqlCommand(
                     "UPDATE FAQEntry SET was_not_helpful_votes = was_not_helpful_votes + 1 WHERE FAQentry_id = @id");
            command.Parameters.AddWithValue("@id", id);

            ExecuteNonQuery(command);
            InvalidateCacheEntry(id);
        }

    protected override FAQEntry MapRowToEntity(SqlDataReader reader)
        {
            int id = (int)reader["FAQentry_id"];
            string question = reader["question"].ToString();
            string answer = reader["answer"].ToString();
            FAQCategoryEnum category = Enum.Parse<FAQCategoryEnum>(reader["category"].ToString());
            int viewCount = (int)reader["view_count"];
            int helpfulVotesCount = (int)reader["was_helpful_votes"];
            int notHelpfulVotesCount = (int)reader["was_not_helpful_votes"];

            return new FAQEntry(id, question, answer, category, viewCount, helpfulVotesCount, notHelpfulVotesCount);
        }

    protected override int GetEntityId(FAQEntry entity)
    {
            return entity.Id;
    }
}
}