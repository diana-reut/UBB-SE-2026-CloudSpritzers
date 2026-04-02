using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service;
using CloudSpritzers1.src.viewModel;
using Microsoft.UI;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudSpritzers1.src.view.ticket
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TicketEmployeeView : Page
    {
        public TicketsViewModel ViewModel { get; }
        public TicketEmployeeView()
        {   
            ViewModel = (App.Current as App).Services.GetService<TicketsViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private async void EditTicketStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int ticketId)
            {
                var ticket = ViewModel.TicketsRead.FirstOrDefault(t => t.TicketId == ticketId);
                if (ticket == null) return;

                var primaryButtonStyle = new Style(typeof(Button));
                primaryButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))));
                primaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.White)));
                primaryButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x2B, 0xB8, 0xC0))));
                primaryButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Microsoft.UI.Xaml.Thickness(1)));
                primaryButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(5))); // Rounded corners

                var closeButtonStyle = new Style(typeof(Button));
                closeButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xE5, 0xE7, 0xEB))));
                closeButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.Black)));
                closeButtonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xE5, 0xE7, 0xEB))));
                closeButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Microsoft.UI.Xaml.Thickness(1)));
                closeButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(5))); // Rounded corners
                var dialog = new ContentDialog
                {
                    Title = $"Edit Status for Ticket #{ticket.TicketId}",
                    PrimaryButtonText = "Save",
                    CloseButtonText = "Cancel",
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = ElementTheme.Light,
                    PrimaryButtonStyle = primaryButtonStyle,
                    CloseButtonStyle = closeButtonStyle
                };


                var combo = new ComboBox
                {
                    Width = 200,
                    Margin = new Thickness(0, 20, 0, 0),
                    RequestedTheme = ElementTheme.Light,
                    Background = new SolidColorBrush(Colors.White),
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
                };

                // Add enum items as strings
                foreach (var status in Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>())
                {
                    combo.Items.Add(status.ToString());
                }

                // Set the selected item to current status
                combo.SelectedItem = ticket.Status.ToString();

                dialog.Content = combo;

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary && combo.SelectedItem is string selectedStr)
                {
                    if (Enum.TryParse<StatusEnum>(selectedStr, out var newStatus))
                    {
                        ViewModel.UpdateStatus(ticket.TicketId, newStatus);
                    }
                }
            }
        }

        // Optional: if you still have a filter combobox
        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            var combo = sender as ComboBox;

            if (combo?.SelectedItem is not ComboBoxItem selected)
                return;

            if (selected.Tag == null)
                return;

            ViewModel.SelectedFilterString = selected.Tag.ToString();

        }

    }
}
