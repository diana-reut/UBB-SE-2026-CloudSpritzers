using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Microsoft.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CloudSpritzers1.src.viewModel;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model.ticket;
using Microsoft.UI.Xaml.Media;

namespace CloudSpritzers1.src.view.ticket
{
    public sealed partial class TicketsView : Page
    {
        private const int DEFAULT_GUEST_IDENTIFIER = 1;
        private const string DEFAULT_SYSTEM_EMAIL = "email@email.com";

        public TicketsViewModel ViewModel { get; }

        public TicketsView()
        {
            ViewModel = (App.Current as App).Services.GetService<TicketsViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }

        private async void CreateTicketButton_Click(object sender, RoutedEventArgs e)
        {
            // Build the UI using the helper method to keep this handler clean
            var (layout, inputs) = BuildSubmissionForm();

            ContentDialog submissionDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Content = new ScrollViewer { MaxHeight = 500, Content = layout },
                Background = new SolidColorBrush(Colors.White),
                RequestedTheme = ElementTheme.Light
            };

            // Logic for the Send button
            inputs.SubmitButton.Click += async (s, args) =>
            {
                await HandleSubmission(inputs, submissionDialog);
            };

            // Logic for the Cancel button
            inputs.CancelButton.Click += (s, args) => submissionDialog.Hide();

            await submissionDialog.ShowAsync();
        }

        private async Task HandleSubmission(SubmissionFormInputs inputs, ContentDialog dialog)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputs.TitleBox.Text) || string.IsNullOrWhiteSpace(inputs.DescriptionBox.Text))
                    throw new Exception("Please fill all required fields.");

                var selectedCategory = ViewModel.Categories.FirstOrDefault(c => c.CategoryName == inputs.CategoryCombo.SelectedItem?.ToString());
                var selectedSubcategory = ViewModel.Subcategories.FirstOrDefault(s => s.SubcategoryName == inputs.SubcategoryCombo.SelectedItem?.ToString());

                var newTicket = new TicketDTO(
                    TicketId: ViewModel.GetTotalTicketCount() + 1,
                    CreatorAccountId: DEFAULT_GUEST_IDENTIFIER,
                    CreatorEmailAddress: DEFAULT_SYSTEM_EMAIL,
                    UrgencyLevel: TicketUrgencyLevelEnum.LOW,
                    CurrentStatus: TicketStatusEnum.OPEN,
                    CategoryId: selectedCategory?.CategoryId ?? 1,
                    CategoryName: selectedCategory?.CategoryName ?? "General",
                    SubcategoryId: selectedSubcategory?.SubcategoryId ?? 1,
                    SubcategoryName: selectedSubcategory?.SubcategoryName ?? "General",
                    Subject: inputs.TitleBox.Text,
                    Description: inputs.DescriptionBox.Text,
                    CreationTimestamp: DateTime.Now
                );

                ViewModel.CreateTicket(newTicket);
                dialog.Hide();
            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
            }
        }

        private (StackPanel Layout, SubmissionFormInputs Inputs) BuildSubmissionForm()
        {
            var panel = new StackPanel { Spacing = 12, Padding = new Thickness(10) };

            panel.Children.Add(new TextBlock { Text = "Submit a New Ticket", FontSize = 24, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold });

            var titleBox = new TextBox { Header = "Title of the issue*", PlaceholderText = "e.g. Damaged suitcase", Width = 400 };
            panel.Children.Add(titleBox);

            var categoryCombo = new ComboBox { Header = "Category*", Width = 400, PlaceholderText = "Select Category" };
            foreach (var cat in ViewModel.Categories) categoryCombo.Items.Add(cat.CategoryName);
            panel.Children.Add(categoryCombo);

            var subcategoryCombo = new ComboBox { Header = "Subcategory*", Width = 400, PlaceholderText = "Select Subcategory" };
            panel.Children.Add(subcategoryCombo);

            categoryCombo.SelectionChanged += (s, e) => {
                subcategoryCombo.Items.Clear();
                var cat = ViewModel.Categories.FirstOrDefault(c => c.CategoryName == categoryCombo.SelectedItem?.ToString());
                if (cat == null) return;
                ViewModel.LoadSubcategories(cat.CategoryId);
                foreach (var sub in ViewModel.Subcategories) subcategoryCombo.Items.Add(sub.SubcategoryName);
            };

            var descriptionBox = new TextBox { Header = "Description*", PlaceholderText = "Details...", Height = 120, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true };
            panel.Children.Add(descriptionBox);

            var btnPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Margin = new Thickness(0, 10, 0, 0) };
            var sendBtn = new Button { Content = "Send", Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 43, 184, 192)), Foreground = new SolidColorBrush(Colors.White) };
            var cancelBtn = new Button { Content = "Cancel" };
            btnPanel.Children.Add(sendBtn);
            btnPanel.Children.Add(cancelBtn);
            panel.Children.Add(btnPanel);

            return (panel, new SubmissionFormInputs { TitleBox = titleBox, CategoryCombo = categoryCombo, SubcategoryCombo = subcategoryCombo, DescriptionBox = descriptionBox, SubmitButton = sendBtn, CancelButton = cancelBtn });
        }

        private async Task ShowError(string message)
        {
            var dialog = new ContentDialog { XamlRoot = this.XamlRoot, Title = "Error", Content = message, CloseButtonText = "OK" };
            await dialog.ShowAsync();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem selected && selected.Tag != null)
            {
                ViewModel.SelectedFilterString = selected.Tag.ToString();
            }
        }
    }

    public class SubmissionFormInputs
    {
        public TextBox TitleBox { get; set; }
        public ComboBox CategoryCombo { get; set; }
        public ComboBox SubcategoryCombo { get; set; }
        public TextBox DescriptionBox { get; set; }
        public Button SubmitButton { get; set; }
        public Button CancelButton { get; set; }
    }
}