using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CloudSpritzers1.Src.View.Chat;
using CloudSpritzers1.Src.View.General;
using CloudSpritzers1.Src.ViewModel.General;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CloudSpritzers1.Src.View.General
{
    public sealed partial class EnterYourId : Page
    {
        /// <summary>
        /// The ViewModel containing user input and authentication logic.
        /// </summary>
        public EnterYourIdViewModel ViewModel { get; } = new ();

        /// <summary>
        /// Initializes a new instance of the EnterYourId page and sets the DataContext.
        /// </summary>
        public EnterYourId()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// Displays an error dialog with the specified message and title.
        /// </summary>
        /// <param name="messageContent">The error message to display.</param>
        /// <param name="titleText">The title of the error dialog.</param>
        private async void DisplayErrorMessage(string messageContent, string titleText)
        {
            var errorDialog = new MaiBoule(messageContent, titleText);
            errorDialog.XamlRoot = this.Content.XamlRoot;
            await errorDialog.ShowAsync();
        }

        /// <summary>
        /// Handles the login button click event.
        /// Validates the user input, shows a confirmation dialog, and attempts authentication.
        /// Navigates to the LandingPage on success or displays an error on failure.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="eventArguments">Event data for the click event. </param>
        private async void LoginButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            // The View handles UI Flow, the ViewModel handles the Data
            if (int.TryParse(ViewModel.UserIdentification, out int parsedId))
            {
                var confirmationDialog = new YouSure($"Are you certain you are ID {parsedId}?", "Confirmation");
                confirmationDialog.XamlRoot = this.Content.XamlRoot;

                if (await confirmationDialog.ShowAsync() == ContentDialogResult.Primary)
                {
                    if (ViewModel.TryAuthenticate(out _))
                    {
                        this.Frame.Navigate(typeof(LandingPage));
                    }
                    else
                    {
                        DisplayErrorMessage("The ID entered does not exist.", "ERROR");
                    }
                }
            }
            else
            {
                DisplayErrorMessage("Please enter a valid numeric ID.", "FORMAT ERROR");
            }
        }

        /// <summary>
        /// Handles the back button click event.
        /// Navigates back if possible, or to the ChoosingPage if not.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="eventArguments">Event data for the click event.</param>
        private void BackButton_Click(object sender, RoutedEventArgs eventArguments)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                this.Frame.Navigate(typeof(ChoosingPage));
            }
        }
    }
}