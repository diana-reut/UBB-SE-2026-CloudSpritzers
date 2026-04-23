
using CloudSpritzers1.src.viewModel.general;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.src.view.general
{
    /// <summary>
    /// A ContentDialog for displaying a message and title to the user.
    /// Uses MaiBouleViewModel for data binding.
    /// </summary>
    /// 
    public sealed partial class MaiBoule : ContentDialog
    {

        /// <summary>
        /// The ViewModel containing the dialog's title and message.
        /// </summary>
        public MaiBouleViewModel ViewModel { get; } // Make getter-only

        /// <summary>
        /// Initializes a new instance of the MaiBoule dialog with default values.
        /// </summary>
        public MaiBoule()
        {
            ViewModel = new MaiBouleViewModel();
            this.InitializeComponent();
            this.DataContext = this; // Ensures x:Bind works for ViewModel
        }

        /// <summary>
        /// Initializes a new instance of the MaiBoule dialog with a custom message and title.
        /// </summary>
        /// <param name="errorMessage">The message to display in the dialog.</param>
        /// <param name="textTitle">The title to display in the dialog. Defaults to Warning </param>
        public MaiBoule(string errorMessage, string textTitle = "Warning") : this()
        {
            ViewModel.Message = errorMessage;
            ViewModel.Title = textTitle;
        }
    }
}
