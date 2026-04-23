using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CloudSpritzers1.Src.Model.Faq.Bot;
using Microsoft.Data.SqlClient;

namespace CloudSpritzers1.Src.Repository.Database
{
    public class DecisionTreeRepository : DatabaseRepository<int, FAQNode>, IRepository<int, FAQNode>
    {
        private ImmutableArray<FAQOption> RetrieveAllAvailableFAQOptionsAssociatedWithSpecificDecisionNodeFromDatabase(int uniqueDatabaseIdentifierForTargetDecisionNode)
        {
            using var activeDatabaseConnectionForExecutingCurrentQuery = CreateConnection();
            activeDatabaseConnectionForExecutingCurrentQuery.Open();

            using var sqlCommandObjectForRetrievingOptions = new SqlCommand(
                "SELECT label, next_option_id FROM FAQOption WHERE node_id = @NodeId", activeDatabaseConnectionForExecutingCurrentQuery);
            sqlCommandObjectForRetrievingOptions.Parameters.AddWithValue("@NodeId", uniqueDatabaseIdentifierForTargetDecisionNode);

            using var sqlDataReaderForParsingReturnedDatabaseRows = sqlCommandObjectForRetrievingOptions.ExecuteReader();
            var listOfRetrievedFAQOptionsForCurrentNode = new List<FAQOption>();
            while (sqlDataReaderForParsingReturnedDatabaseRows.Read())
            {
                listOfRetrievedFAQOptionsForCurrentNode.Add(new FAQOption(
                    sqlDataReaderForParsingReturnedDatabaseRows.GetString(sqlDataReaderForParsingReturnedDatabaseRows.GetOrdinal("label")),
                    sqlDataReaderForParsingReturnedDatabaseRows.GetInt32(sqlDataReaderForParsingReturnedDatabaseRows.GetOrdinal("next_option_id"))));
            }
            return listOfRetrievedFAQOptionsForCurrentNode.ToImmutableArray();
        }

        private Dictionary<int, ImmutableArray<FAQOption>> RetrieveAndGroupAllFAQOptionsAvailableInTheEntireDatabase()
        {
            using var activeDatabaseConnectionForExecutingCurrentQuery = CreateConnection();
            activeDatabaseConnectionForExecutingCurrentQuery.Open();

            using var sqlCommandObjectForRetrievingAllOptions = new SqlCommand(
                "SELECT node_id, label, next_option_id FROM FAQOption", activeDatabaseConnectionForExecutingCurrentQuery);

            using var sqlDataReaderForParsingReturnedDatabaseRows = sqlCommandObjectForRetrievingAllOptions.ExecuteReader();
            var dictionaryMappingNodeIdentifiersToTheirRespectiveFAQOptions = new Dictionary<int, List<FAQOption>>();

            while (sqlDataReaderForParsingReturnedDatabaseRows.Read())
            {
                int uniqueDatabaseIdentifierForCurrentFAQNode = sqlDataReaderForParsingReturnedDatabaseRows.GetInt32(sqlDataReaderForParsingReturnedDatabaseRows.GetOrdinal("node_id"));
                var currentlyIteratedFAQOptionFromDatabase = new FAQOption(
                    sqlDataReaderForParsingReturnedDatabaseRows.GetString(sqlDataReaderForParsingReturnedDatabaseRows.GetOrdinal("label")),
                    sqlDataReaderForParsingReturnedDatabaseRows.GetInt32(sqlDataReaderForParsingReturnedDatabaseRows.GetOrdinal("next_option_id")));

                if (!dictionaryMappingNodeIdentifiersToTheirRespectiveFAQOptions.ContainsKey(uniqueDatabaseIdentifierForCurrentFAQNode))
                {
                    dictionaryMappingNodeIdentifiersToTheirRespectiveFAQOptions[uniqueDatabaseIdentifierForCurrentFAQNode] = new List<FAQOption>();
                }
                dictionaryMappingNodeIdentifiersToTheirRespectiveFAQOptions[uniqueDatabaseIdentifierForCurrentFAQNode].Add(currentlyIteratedFAQOptionFromDatabase);
            }

            return dictionaryMappingNodeIdentifiersToTheirRespectiveFAQOptions.ToDictionary(
                keyValuePairHoldingNodeIdAndOptionList => keyValuePairHoldingNodeIdAndOptionList.Key,
                keyValuePairHoldingNodeIdAndOptionList => keyValuePairHoldingNodeIdAndOptionList.Value.ToImmutableArray());
        }

        public FAQNode GetById(int id)
        {
            using var sqlCommandObjectForRetrievingSpecificFAQNode = new SqlCommand(
                "SELECT node_id, question_text, is_final_answer FROM FAQNode WHERE node_id = @Id");
            sqlCommandObjectForRetrievingSpecificFAQNode.Parameters.AddWithValue("@Id", id);

            var retrievedFAQNodeEntityFromBaseRepository = GetById(id, sqlCommandObjectForRetrievingSpecificFAQNode);
            if (retrievedFAQNodeEntityFromBaseRepository == null)
            {
                return null;
            }

            var immutableArrayOfOptionsForThisNode = RetrieveAllAvailableFAQOptionsAssociatedWithSpecificDecisionNodeFromDatabase(id);
            return retrievedFAQNodeEntityFromBaseRepository with { options = immutableArrayOfOptionsForThisNode };
        }

        public int CreateNewEntity(FAQNode incomingFAQNodeEntityToBeSaved)
        {
            using var sqlCommandForInsertingNewFAQNodeIntoDatabase = new SqlCommand(@"
                INSERT INTO FAQNode (question_text, is_final_answer)
                OUTPUT INSERTED.node_id
                VALUES (@QuestionText, @IsFinalAnswer)");

            sqlCommandForInsertingNewFAQNodeIntoDatabase.Parameters.AddWithValue("@QuestionText", incomingFAQNodeEntityToBeSaved.questionText);
            sqlCommandForInsertingNewFAQNodeIntoDatabase.Parameters.AddWithValue("@IsFinalAnswer", incomingFAQNodeEntityToBeSaved.isFinalAnswer);

            int newlyGeneratedDatabaseIdentifierForCreatedFAQNode = Add(sqlCommandForInsertingNewFAQNodeIntoDatabase, incomingFAQNodeEntityToBeSaved);

            foreach (var currentlyIteratedFAQOptionToInsert in incomingFAQNodeEntityToBeSaved.options)
            {
                using var sqlCommandForInsertingNewFAQOptionIntoDatabase = new SqlCommand(@"
                    INSERT INTO FAQOption (node_id, label, next_option_id)
                    VALUES (@NodeId, @Label, @NextOptionId)");

                sqlCommandForInsertingNewFAQOptionIntoDatabase.Parameters.AddWithValue("@NodeId", newlyGeneratedDatabaseIdentifierForCreatedFAQNode);
                sqlCommandForInsertingNewFAQOptionIntoDatabase.Parameters.AddWithValue("@Label", currentlyIteratedFAQOptionToInsert.label);
                sqlCommandForInsertingNewFAQOptionIntoDatabase.Parameters.AddWithValue("@NextOptionId", currentlyIteratedFAQOptionToInsert.nextOptionId);

                ExecuteNonQuery(sqlCommandForInsertingNewFAQOptionIntoDatabase);
            }

            return newlyGeneratedDatabaseIdentifierForCreatedFAQNode;
        }

        public void DeleteById(int identifierForFAQNodeToBeDeleted)
        {
            using var sqlCommandForRemovingAllFAQOptionsAssociatedWithNode = new SqlCommand(
                "DELETE FROM FAQOption WHERE node_id = @Id");
            sqlCommandForRemovingAllFAQOptionsAssociatedWithNode.Parameters.AddWithValue("@Id", identifierForFAQNodeToBeDeleted);
            ExecuteNonQuery(sqlCommandForRemovingAllFAQOptionsAssociatedWithNode);

            using var sqlCommandForRemovingSpecificFAQNodeFromDatabase = new SqlCommand(
                "DELETE FROM FAQNode WHERE node_id = @Id");
            sqlCommandForRemovingSpecificFAQNodeFromDatabase.Parameters.AddWithValue("@Id", identifierForFAQNodeToBeDeleted);
            DeleteById(identifierForFAQNodeToBeDeleted, sqlCommandForRemovingSpecificFAQNodeFromDatabase);
        }

        public void UpdateById(int identifierForFAQNodeToBeUpdated, FAQNode updatedFAQNodeEntityData)
        {
            using var sqlCommandForUpdatingSpecificFAQNodeInDatabase = new SqlCommand(@"
                UPDATE FAQNode
                SET question_text = @QuestionText,
                    is_final_answer = @IsFinalAnswer
                WHERE node_id = @Id");

            sqlCommandForUpdatingSpecificFAQNodeInDatabase.Parameters.AddWithValue("@Id", identifierForFAQNodeToBeUpdated);
            sqlCommandForUpdatingSpecificFAQNodeInDatabase.Parameters.AddWithValue("@QuestionText", updatedFAQNodeEntityData.questionText);
            sqlCommandForUpdatingSpecificFAQNodeInDatabase.Parameters.AddWithValue("@IsFinalAnswer", updatedFAQNodeEntityData.isFinalAnswer);

            UpdateById(identifierForFAQNodeToBeUpdated, sqlCommandForUpdatingSpecificFAQNodeInDatabase, updatedFAQNodeEntityData);

            using var sqlCommandForRemovingAllOldFAQOptionsAssociatedWithNode = new SqlCommand(
                "DELETE FROM FAQOption WHERE node_id = @Id");
            sqlCommandForRemovingAllOldFAQOptionsAssociatedWithNode.Parameters.AddWithValue("@Id", identifierForFAQNodeToBeUpdated);
            ExecuteNonQuery(sqlCommandForRemovingAllOldFAQOptionsAssociatedWithNode);

            foreach (var currentlyIteratedFAQOptionToInsertAsReplacement in updatedFAQNodeEntityData.options)
            {
                using var sqlCommandForInsertingReplacementFAQOptionIntoDatabase = new SqlCommand(@"
                    INSERT INTO FAQOption (node_id, label, next_option_id)
                    VALUES (@NodeId, @Label, @NextOptionId)");

                sqlCommandForInsertingReplacementFAQOptionIntoDatabase.Parameters.AddWithValue("@NodeId", identifierForFAQNodeToBeUpdated);
                sqlCommandForInsertingReplacementFAQOptionIntoDatabase.Parameters.AddWithValue("@Label", currentlyIteratedFAQOptionToInsertAsReplacement.label);
                sqlCommandForInsertingReplacementFAQOptionIntoDatabase.Parameters.AddWithValue("@NextOptionId", currentlyIteratedFAQOptionToInsertAsReplacement.nextOptionId);

                ExecuteNonQuery(sqlCommandForInsertingReplacementFAQOptionIntoDatabase);
            }
        }

        public IEnumerable<FAQNode> GetAll()
        {
            using var sqlCommandForRetrievingAllFAQNodesFromDatabase = new SqlCommand(
                "SELECT node_id, question_text, is_final_answer FROM FAQNode");

            var listOfAllRetrievedFAQNodesFromDatabase = GetAll(sqlCommandForRetrievingAllFAQNodesFromDatabase).ToList();

            var comprehensiveDictionaryOfAllFAQOptionsMappedByNodeId = RetrieveAndGroupAllFAQOptionsAvailableInTheEntireDatabase();

            return listOfAllRetrievedFAQNodesFromDatabase.Select(currentlyIteratedFAQNode =>
                currentlyIteratedFAQNode with
                {
                    options = comprehensiveDictionaryOfAllFAQOptionsMappedByNodeId.TryGetValue(currentlyIteratedFAQNode.faqNodeId, out var correspondingOptionsForThisNode)
                        ? correspondingOptionsForThisNode
                        : ImmutableArray<FAQOption>.Empty
                }).ToList();
        }

        protected override FAQNode MapRowToEntity(SqlDataReader sqlDataReaderContainingDatabaseRowData)
        {
            return new FAQNode(
                sqlDataReaderContainingDatabaseRowData.GetInt32(sqlDataReaderContainingDatabaseRowData.GetOrdinal("node_id")),
                sqlDataReaderContainingDatabaseRowData.GetString(sqlDataReaderContainingDatabaseRowData.GetOrdinal("question_text")),
                new ImmutableArray<FAQOption>(),
                sqlDataReaderContainingDatabaseRowData.GetBoolean(sqlDataReaderContainingDatabaseRowData.GetOrdinal("is_final_answer")));
        }

        protected override int GetEntityId(FAQNode specificFAQNodeEntity) => specificFAQNodeEntity.faqNodeId;
    }
}