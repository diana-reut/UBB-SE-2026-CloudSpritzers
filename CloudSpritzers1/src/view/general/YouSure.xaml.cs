using CloudSpritzers1.src.viewModel.general;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;


namespace CloudSpritzers1.src.view.general
{
    public sealed partial class YouSure : ContentDialog
    {
        // Change to a read-only getter to prevent accidental reassignment
        public YouSureViewModel ViewModel { get; } = new();

        public YouSure()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Initializes a new instance of the confirmation dialog with semantic parameters.
        /// </summary>
        /// <param name="messageContent">The description shown to the user.</param>
        /// <param name="titleText">The heading of the dialog.</param>
        public YouSure(string messageContent, string titleText = "Confirm") : this()
        {
            
            ViewModel.Message = messageContent;
            ViewModel.Title = titleText;
        }
    }
}