using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Dto.MappingProfiles;
using CloudSpritzers1.Src.Model.Faq;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Implementation;
using CloudSpritzers1.Src.Service.Implementation;
using CloudSpritzers1.Src.ViewModel.Faq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace CloudSpritzers1.Src.View.Faq
{
    public sealed partial class FAQAddEditPage : Page
    {
        private FAQViewModel viewModel;
        private FAQEntryDTO? editingFaq;
        private bool isEditMode;
        private int currentPersonId;

        public FAQAddEditPage()
        {
            this.InitializeComponent();

            var app = (App)Application.Current;
            viewModel = app.Services.GetRequiredService<FAQViewModel>();
        }

        // protected override void OnNavigatedTo(NavigationEventArgs e)
        // {
        //    base.OnNavigatedTo(e);
        //    if (e.Parameter is FAQEntryDTO faq)
        //    {
        //        _editingFaq = faq;
        //        _isEditMode = true;
        //        QuestionTextBox.Text = faq.Question;
        //        AnswerTextBox.Text = faq.Answer;
        //        CategoryComboBox.SelectedItem = FindCategoryComboBoxItem(faq.Category);
        //        PageTitleText.Text = "Edit FAQ";
        //        PageSubtitleText.Text = "Update the selected frequently asked question entry";
        //        SaveButton.Content = "Save Changes";
        //    }
        //    else
        //    {
        //        _editingFaq = null;
        //        _isEditMode = false;
        //        PageTitleText.Text = "Add FAQ";
        //        PageSubtitleText.Text = "Create a frequently asked question entry";
        //        SaveButton.Content = "Add FAQ";
        //    }
        // }
        // protected override void OnNavigatedTo(NavigationEventArgs e)
        // {
        //    base.OnNavigatedTo(e);
        //    if (e.Parameter is FAQNavigationData navData)
        //    {
        //        _currentPersonId = navData.CurrentPersonId;
        //        _viewModel.IsAdmin = IsEmployee(_currentPersonId);
        //        if (navData.FAQEntry != null)
        //        {
        //            var faq = navData.FAQEntry;
        //            _editingFaq = faq;
        //            _isEditMode = true;
        //            QuestionTextBox.Text = faq.Question;
        //            AnswerTextBox.Text = faq.Answer;
        //            CategoryComboBox.SelectedItem = FindCategoryComboBoxItem(faq.Category);
        //            PageTitleText.Text = "Edit FAQ";
        //            PageSubtitleText.Text = "Update the selected frequently asked question entry";
        //            SaveButton.Content = "Save Changes";
        //        }
        //        else
        //        {
        //            _editingFaq = null;
        //            _isEditMode = false;
        //            QuestionTextBox.Text = string.Empty;
        //            AnswerTextBox.Text = string.Empty;
        //            CategoryComboBox.SelectedItem = null;
        //            PageTitleText.Text = "Add FAQ";
        //            PageSubtitleText.Text = "Create a frequently asked question entry";
        //            SaveButton.Content = "Add FAQ";
        //        }
        //    }
        // }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FAQNavigationData navData)
            {
                currentPersonId = navData.CurrentPersonId;
                viewModel.IsAdmin = navData.IsEmployee;

                if (navData.FAQEntry != null)
                {
                    editingFaq = navData.FAQEntry;
                    isEditMode = true;
                    viewModel.SelectedFAQEntry = navData.FAQEntry;

                    QuestionTextBox.Text = editingFaq.Question;
                    AnswerTextBox.Text = editingFaq.Answer;
                    CategoryComboBox.SelectedItem = FindCategoryComboBoxItem(editingFaq.Category);

                    PageTitleText.Text = "Edit FAQ";
                    PageSubtitleText.Text = "Update the selected frequently asked question entry";
                    SaveButton.Content = "Save Changes";
                }
                else
                {
                    editingFaq = null;
                    isEditMode = false;
                    viewModel.SelectedFAQEntry = null;

                    QuestionTextBox.Text = string.Empty;
                    AnswerTextBox.Text = string.Empty;
                    CategoryComboBox.SelectedItem = null;

                    PageTitleText.Text = "Add FAQ";
                    PageSubtitleText.Text = "Create a frequently asked question entry";
                    SaveButton.Content = "Add FAQ";
                }
            }
        }

        private ComboBoxItem? FindCategoryComboBoxItem(FAQCategoryEnum category)
        {
            foreach (var item in CategoryComboBox.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    comboItem.Content?.ToString() == category.ToString())
                {
                    return comboItem;
                }
            }

            return null;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await HandleSaveChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async System.Threading.Tasks.Task HandleSaveChanges()
        {
            try
            {
                await viewModel.Save(
                    QuestionTextBox.Text,
                    AnswerTextBox.Text,
                    (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString());

                if (Frame != null && Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                await ShowMessage("Save failed", ex.Message);
            }
        }
        private async System.Threading.Tasks.Task ShowMessage(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
