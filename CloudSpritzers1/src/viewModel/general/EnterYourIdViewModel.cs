using CommunityToolkit.Mvvm.ComponentModel;


/// <summary>
/// ViewModel for the EnterYourId page.
/// Handles user identification input and authentication logic.
/// </summary>
/// 
namespace CloudSpritzers1.src.viewModel.general
{
    public sealed partial class EnterYourIdViewModel : ObservableObject
    {
        /// <summary>
        /// The user-provided identification string.
        /// </summary>
        [ObservableProperty]
        private string _userIdentification;

        /// <summary>
        /// Attempts to authenticate the user by parsing the identification string
        /// and calling the application's SetUser method.
        /// </summary>
        /// <param name="parsedId">The parsed integer user ID if successful.</param>
        /// <returns>True if authentication succeeds; otherwise, false.</returns>
        public bool TryAuthenticate(out int parsedId)
        {
            if (int.TryParse(UserIdentification, out parsedId))
            {
                return ((App)App.Current).SetUser(parsedId);
            }
            return false;
        }
    }
}