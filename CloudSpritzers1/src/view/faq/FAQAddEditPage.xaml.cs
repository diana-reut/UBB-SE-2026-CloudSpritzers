using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.dto.mappingProfiles;
using CloudSpritzers1.src.model.faq;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.implementations;
using CloudSpritzers1.src.service.implementation;
using CloudSpritzers1.src.viewModel.faq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudSpritzers1.src.view.faq
{
   
    public sealed partial class FAQAddEditPage : Page
    {

        private FAQViewModel _viewModel;
        private FAQEntryDTO? _editingFaq;
        private bool _isEditMode;
        private int _currentPersonId;


        public FAQAddEditPage()
        {
            this.InitializeComponent();

            var app = (App)Application.Current;
            _viewModel = app.Services.GetRequiredService<FAQViewModel>();

        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
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
        //}

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
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
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is FAQNavigationData navData)
            {
                _currentPersonId = navData.CurrentPersonId;
                _viewModel.IsAdmin = navData.IsEmployee;

                if (navData.FAQEntry != null)
                {
                    _editingFaq = navData.FAQEntry;
                    _isEditMode = true;
                    _viewModel.SelectedFAQEntry = navData.FAQEntry;

                    QuestionTextBox.Text = _editingFaq.Question;
                    AnswerTextBox.Text = _editingFaq.Answer;
                    CategoryComboBox.SelectedItem = FindCategoryComboBoxItem(_editingFaq.Category);

                    PageTitleText.Text = "Edit FAQ";
                    PageSubtitleText.Text = "Update the selected frequently asked question entry";
                    SaveButton.Content = "Save Changes";
                }
                else
                {
                    _editingFaq = null;
                    _isEditMode = false;
                    _viewModel.SelectedFAQEntry = null;

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
                await _viewModel.Save(
                    QuestionTextBox.Text,
                    AnswerTextBox.Text,
                    (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                );

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
