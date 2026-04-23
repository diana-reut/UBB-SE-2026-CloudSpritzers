using CloudSpritzers1.src.model.message;

namespace CloudSpritzers1.src.model
{
    public class User : ISender
    {
        private int _userId;
        private string _fullName;
        private string _emailAddress;

        public User(int userId, string fullName, string emailAddress)
        {
            _userId = userId;
            _fullName = fullName;
            _emailAddress = emailAddress;
        }

        public int UserId => _userId;

        public string RetrieveConfiguredDisplayFullNameForBot() => _fullName;
        public string RetrieveConfiguredEmailAddressForBotContact() => _emailAddress;

        public int RetrieveUniqueDatabaseIdentifierForBot() => _userId;
    }
}
