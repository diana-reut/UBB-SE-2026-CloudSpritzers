using CommunityToolkit.Mvvm.ComponentModel;

namespace CloudSpritzers1.Src.ViewModel.General
{
    /// <summary>
    /// ViewModel for the MaiBoule dialog.
    /// Provides properties for the dialog's title and message, with change notification support.
    /// </summary>
    public sealed partial class MaiBouleViewModel : ObservableObject
    {
        /// <summary>
        /// The title displayed in the dialog.
        /// Default value is "Warning".
        /// </summary>
        [ObservableProperty]
        private string title = "Warning";

        /// <summary>
        /// The message content displayed in the dialog.
        /// Default value is "Oopsie Daisy! Boule.".
        /// </summary>
        [ObservableProperty]
        private string message = "Oopsie Daisy! Boule.";
    }
}
