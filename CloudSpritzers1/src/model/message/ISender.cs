namespace CloudSpritzers1.Src.Model.Message
{
    public interface ISender
    {
        int RetrieveUniqueDatabaseIdentifierForBot();
        string RetrieveConfiguredDisplayFullNameForBot();
        string RetrieveConfiguredEmailAddressForBotContact();
    }
}
