namespace CloudSpritzers1.src.model.message
{
    public interface ISender
    {
        int RetrieveUniqueDatabaseIdentifierForBot();
        string RetrieveConfiguredDisplayFullNameForBot();
        string RetrieveConfiguredEmailAddressForBotContact();
    }
}
