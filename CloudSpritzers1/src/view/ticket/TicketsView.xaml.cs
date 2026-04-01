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
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using CloudSpritzers1.src.model.ticket;
using CloudSpritzers1.src.model;
using System.Collections.ObjectModel;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.service;
using CloudSpritzers1.src.viewModel;

namespace CloudSpritzers1.src.view
{
    public sealed partial class TicketsView : Page
    {
        public TicketsViewModel ViewModel { get; }

        public TicketsView()
        {
            this.InitializeComponent();
            var service = new TicketService(new TicketRepository());
            ViewModel = new TicketsViewModel(service);
            this.DataContext = ViewModel;

            // Bind ItemsControl
            TicketList.ItemsSource = ViewModel.GetAllTickets();
        }
        private async void CreateTicketButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = null;

            // Input controls references
            TextBox titleBox = null;
            ComboBox categoryCombo = null;
            ComboBox subcategoryCombo = null;
            TextBox descriptionBox = null;
            TextBox emailBox = null;

            // Main content panel
            var stackPanel = new StackPanel
            {
                Spacing = 12,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Header
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Submit a New Ticket",
                FontSize = 24,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Please provide detailed information about your issue. All fields marked with * are required.",
                FontSize = 14,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 107, 114, 128)),
                TextWrapping = TextWrapping.Wrap
            });

            // Title
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Title of the issue*",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            titleBox = new TextBox
            {
                PlaceholderText = "e.g., Lost baggage, damaged suitcase, service complaint",
                Width = 400,
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 243, 245)),
                Foreground = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            stackPanel.Children.Add(titleBox);

            // Category
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Category*",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            categoryCombo = new ComboBox
            {
                PlaceholderText = "Select a category",
                Width = 400,
                RequestedTheme = ElementTheme.Light,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            categoryCombo.Items.Add("Baggage");
            categoryCombo.Items.Add("Service");
            categoryCombo.Items.Add("Technical");
            stackPanel.Children.Add(categoryCombo);

            // Subcategory
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Subcategory*",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            subcategoryCombo = new ComboBox
            {
                PlaceholderText = "Select a subcategory",
                Width = 400,
                RequestedTheme = ElementTheme.Light,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            subcategoryCombo.Items.Add("Lost Item");
            subcategoryCombo.Items.Add("Damage");
            subcategoryCombo.Items.Add("Complaint");
            stackPanel.Children.Add(subcategoryCombo);

            // Description
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Description*",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            descriptionBox = new TextBox
            {
                PlaceholderText = "Please describe the issue in detail",
                Width = 400,
                Height = 120,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 243, 245)),
                Foreground = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            stackPanel.Children.Add(descriptionBox);

            // Email
            stackPanel.Children.Add(new TextBlock
            {
                Text = "Email Address*",
                FontSize = 16,
                Foreground = new SolidColorBrush(Colors.Black)
            });

            emailBox = new TextBox
            {
                PlaceholderText = "your.email@example.com",
                Width = 400,
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 243, 245)),
                Foreground = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(8),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            stackPanel.Children.Add(emailBox);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Spacing = 12,
                Margin = new Thickness(0, 12, 0, 0)
            };

            var sendBtn = new Button
            {
                Content = "Send",
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192)),
                Foreground = new SolidColorBrush(Colors.White),
                Padding = new Thickness(12, 6, 12, 6)
            };
            sendBtn.Click += async (s, args) =>
            {
                try
                {
                    StatusEnum Status = StatusEnum.OPEN;
                    UrgencyLevelEnum UrgencyLevel = UrgencyLevelEnum.LOW;

                    int TicketId = ViewModel.NrTickets() + 1;
                    string Subject = titleBox.Text;
                    string Description = descriptionBox.Text;
                    DateTime CreatedAt = DateTime.Now;
                    string UserEmail = emailBox.Text;

                    int UserId = 1;
                    string CategoryName = categoryCombo.SelectedItem?.ToString() ?? "General";
                    int CategoryId = 1;

                    string SubcategoryName = subcategoryCombo.SelectedItem?.ToString() ?? "General";
                    int SubcategoryId = 1;

                    var newTicket = new TicketDTO(
                        TicketId,
                        UserId,
                        UserEmail,
                        UrgencyLevel,
                        Status,
                        CategoryId,
                        CategoryName,
                        SubcategoryId,
                        SubcategoryName,
                        Subject,
                        Description,
                        CreatedAt
                    );

                    ViewModel.CreateTicket(newTicket);

                    dialog?.Hide();
                }
                catch (Exception ex)
                {
                    dialog?.Hide();

                    var closeBtn = new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    };

                    var errorPanel = new StackPanel
                    {
                        Spacing = 12,
                        Width = 320
                    };

                    errorPanel.Children.Add(new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 10,
                        Children =
                            {
                                new FontIcon
                                {
                                    Glyph = "\uEA39", // Error icon
                                    FontFamily = new FontFamily("Segoe MDL2 Assets"),
                                    Foreground = new SolidColorBrush(Colors.Red),
                                    FontSize = 28,
                                    VerticalAlignment = VerticalAlignment.Center
                                },
                                new TextBlock
                                {
                                    Text = "Ticket creation failed :(",
                                    FontSize = 18,
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                            }
                    });

                    errorPanel.Children.Add(new TextBlock
                    {
                        Text = ex.Message,
                        TextWrapping = TextWrapping.Wrap
                    });

                    errorPanel.Children.Add(closeBtn);

                    var errorDialog = new ContentDialog
                    {
                        Content = errorPanel,
                        XamlRoot = this.XamlRoot,
                        DefaultButton = ContentDialogButton.None   // removes bottom bar
                    };

                    closeBtn.Click += (_, _) => errorDialog.Hide();

                    await errorDialog.ShowAsync();
                }
            };


            var cancelBtn = new Button
            {
                Content = "Cancel",
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12, 6, 12, 6)
            };
            cancelBtn.Click += (s, args) => dialog?.Hide();

            buttonPanel.Children.Add(sendBtn);
            buttonPanel.Children.Add(cancelBtn);
            stackPanel.Children.Add(buttonPanel);

            // Create the dialog
            dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Background = new SolidColorBrush(Colors.White),
                RequestedTheme = ElementTheme.Light,
                Content = new ScrollViewer
                {
                    MaxHeight = 500,
                    Content = stackPanel
                },
                PrimaryButtonText = null,
                CloseButtonText = null
            };

            await dialog.ShowAsync();
        }
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