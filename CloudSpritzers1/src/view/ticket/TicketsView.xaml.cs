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
using CloudSpritzers1.src.viewModel.review;
using Microsoft.Extensions.DependencyInjection;

namespace CloudSpritzers1.src.view.ticket
{
    public sealed partial class TicketsView : Page
    {
        public TicketsViewModel ViewModel { get; }
        public TicketsView()
        {
            ViewModel = (App.Current as App).Services.GetService<TicketsViewModel>();
            this.InitializeComponent();


            this.DataContext = ViewModel;

            // Bind ItemsControl
            TicketList.ItemsSource = ViewModel.GetAllTickets();
        }
        private async void CreateTicketButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = null;

            // Input controls
            TextBox titleBox = null;
            ComboBox categoryCombo = null;
            ComboBox subcategoryCombo = null;
            TextBox descriptionBox = null;
            TextBox emailBox = null;

            // Main content panel
            var stackPanel = new StackPanel { Spacing = 12, HorizontalAlignment = HorizontalAlignment.Left };

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
            stackPanel.Children.Add(new TextBlock { Text = "Title of the issue*", FontSize = 16, Foreground = new SolidColorBrush(Colors.Black) });
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
            stackPanel.Children.Add(new TextBlock { Text = "Category*", FontSize = 16, Foreground = new SolidColorBrush(Colors.Black) });
            categoryCombo = new ComboBox
            {
                PlaceholderText = "Select a category",
                Width = 400,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            foreach (var cat in ViewModel.Categories)
                categoryCombo.Items.Add(cat.Name);
            stackPanel.Children.Add(categoryCombo);

            // Subcategory
            stackPanel.Children.Add(new TextBlock { Text = "Subcategory*", FontSize = 16, Foreground = new SolidColorBrush(Colors.Black) });
            subcategoryCombo = new ComboBox
            {
                PlaceholderText = "Select a subcategory",
                Width = 400,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                PlaceholderForeground = new SolidColorBrush(Colors.DarkGray)
            };
            stackPanel.Children.Add(subcategoryCombo);

            // Load subcategories when category changes
            categoryCombo.SelectionChanged += (_, _) =>
            {
                subcategoryCombo.Items.Clear();
                if (categoryCombo.SelectedItem == null) return;
                var selectedCategory = ViewModel.Categories.FirstOrDefault(c => c.Name == categoryCombo.SelectedItem.ToString());
                if (selectedCategory == null) return;
                ViewModel.LoadSubcategories(selectedCategory.CategoryId);
                foreach (var sub in ViewModel.Subcategories)
                    subcategoryCombo.Items.Add(sub.SubcategoryName);
            };

            // Description
            stackPanel.Children.Add(new TextBlock { Text = "Description*", FontSize = 16, Foreground = new SolidColorBrush(Colors.Black) });
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
            stackPanel.Children.Add(new TextBlock { Text = "Email Address*", FontSize = 16, Foreground = new SolidColorBrush(Colors.Black) });
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
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Spacing = 12, Margin = new Thickness(0, 12, 0, 0) };

            var sendBtn = new Button { Content = "Send", Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192)), Foreground = new SolidColorBrush(Colors.White), Padding = new Thickness(12, 6, 12, 6) };
            sendBtn.Click += async (s, args) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(titleBox.Text) || string.IsNullOrWhiteSpace(descriptionBox.Text) || string.IsNullOrWhiteSpace(emailBox.Text))
                        throw new Exception("Please fill all required fields.");

                    // Map Category and Subcategory IDs
                    var selectedCategory = ViewModel.Categories.FirstOrDefault(c => c.Name == categoryCombo.SelectedItem?.ToString());
                    var selectedSubcategory = ViewModel.Subcategories.FirstOrDefault(s => s.SubcategoryName == subcategoryCombo.SelectedItem?.ToString());

                    int categoryId = selectedCategory?.CategoryId ?? 1;
                    int subcategoryId = selectedSubcategory?.SubcategoryId ?? 1;

                    // Create DTO
                    var newTicket = new TicketDTO(
                        TicketId: ViewModel.NrTickets() + 1,
                        UserId: 1,
                        UserEmail: emailBox.Text,
                        UrgencyLevel: UrgencyLevelEnum.LOW,
                        Status: StatusEnum.OPEN,
                        CategoryId: categoryId,
                        CategoryName: selectedCategory?.Name ?? "General",
                        SubcategoryId: subcategoryId,
                        SubcategoryName: selectedSubcategory?.SubcategoryName ?? "General",
                        Subject: titleBox.Text,
                        Description: descriptionBox.Text,
                        CreatedAt: DateTime.Now
                    );

                    ViewModel.CreateTicket(newTicket);
                    dialog?.Hide();
                }
                catch (Exception ex)
                {
                    dialog?.Hide();
                    var errorDialog = new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        Title = "Error",
                        Content = ex.Message,
                        CloseButtonText = "OK"
                    };
                    await errorDialog.ShowAsync();
                }
            };

            var cancelBtn = new Button { Content = "Cancel", Background = new SolidColorBrush(Colors.White), Foreground = new SolidColorBrush(Colors.Black), BorderBrush = new SolidColorBrush(Colors.Black), BorderThickness = new Thickness(1), Padding = new Thickness(12, 6, 12, 6) };
            cancelBtn.Click += (s, args) => dialog?.Hide();

            buttonPanel.Children.Add(sendBtn);
            buttonPanel.Children.Add(cancelBtn);
            stackPanel.Children.Add(buttonPanel);

            // Show dialog
            dialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Background = new SolidColorBrush(Colors.White),
                RequestedTheme = ElementTheme.Light,
                Content = new ScrollViewer { MaxHeight = 500, Content = stackPanel },
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
