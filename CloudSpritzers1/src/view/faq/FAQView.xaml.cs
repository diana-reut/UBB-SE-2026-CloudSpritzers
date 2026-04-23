using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.dto.mappingProfiles;
using CloudSpritzers1.src.model.faq;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.implementations;
using CloudSpritzers1.src.service.implementation;
using CloudSpritzers1.src.viewModel.faq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;


namespace CloudSpritzers1.src.view.faq
{
    public sealed partial class FAQView : Page
    {
        public FAQViewModel ViewModel { get; }

        private int _currentPersonId;


        private bool IsEmployee(int id)
        {
            try
            {
                var employeeRepository = new EmployeeRepository();
                var employee = employeeRepository.GetById(id);
                return employee != null;
            }
            catch
            {
                return false;
            }
        }

       

        public FAQView()
        {
            this.InitializeComponent();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FAQEntryMappingProfile>();
            });

            var mapper = mapperConfig.CreateMapper();
            var repository = new FAQRepository();
            var service = new FAQService(repository);

            //bool isAdmin =true; // set true for testing admin mode
            //ViewModel = new FAQViewModel(service, mapper, isAdmin);
            ViewModel = new FAQViewModel(service, mapper);

            DataContext = ViewModel;

            UpdateAdminVisibility();
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{


        //    base.OnNavigatedTo(e);

        //    if (e.Parameter is FAQNavigationData navData)
        //    {
        //        _currentPersonId = navData.CurrentPersonId;
        //        ViewModel.IsAdmin = navData.IsEmployee;
        //    }
        //    else
        //    {
        //        ViewModel.IsAdmin = false;
        //    }

        //    ViewModel.LoadFAQ();
        //    UpdateAdminVisibility();
        //}


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var app = (App)App.Current;

            ViewModel.IsAdmin = app.isEmployee;

            if (app.isEmployee && app.Employee != null)
                _currentPersonId = app.Employee.RetrieveUniqueDatabaseIdentifierForBot();
            else if (app.User != null)
                _currentPersonId = app.User.RetrieveUniqueDatabaseIdentifierForBot();

            ViewModel.LoadFAQ();
            UpdateAdminVisibility();
        }

        private void OpenFaqButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AccordionHeader_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is FAQEntryDTO faq)
            {
                ViewModel.ToggleFAQ(faq);
                ScrollToMiddleIfExpanded(faq);
            }
        }

        private void AllQuestionsButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.All);
            SetCategoryUI(AllQuestionsButton);
        }

        private void CheckInButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.CheckIn);
            SetCategoryUI(CheckInButton);
        }

        private void ParkingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Parking);
            SetCategoryUI(ParkingButton);
        }

        private void BaggageButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Baggage);
            SetCategoryUI(BaggageButton);
        }

        private void TicketButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Tickets);
            SetCategoryUI(TicketsButton);
        }

        private void FacilitiesButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetCategory(FAQCategoryEnum.Facilities);
            SetCategoryUI(FacilitiesButton);
        }

        private void AddFaqButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new FAQNavigationData
            {
                CurrentPersonId = _currentPersonId,
                IsEmployee = ViewModel.IsAdmin,
                FAQEntry = null
            };

            Frame.Navigate(typeof(FAQAddEditPage), data);
        }


        private void EditFaqButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedFAQEntry == null)
                return;

            var data = ViewModel.BuildNavigationData(_currentPersonId);

            Frame.Navigate(typeof(FAQAddEditPage), data);
        }

        private async void DeleteFaqButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedFAQEntry == null)
                return;

            var faq = ViewModel.SelectedFAQEntry;

            var dialog = new ContentDialog
            {
                Title = "Delete FAQ",
                Content = $"Are you sure you want to delete \"{faq.Question}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.DeleteFAQEntry(faq);
                ViewModel.SelectedFAQEntry = null;
            }
        }

        private void HelpfulButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is FAQEntryDTO faq)
            {
                ViewModel.GiveFeedback(faq, true);
            }
        }

        private void NotHelpfulButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is FAQEntryDTO faq)
            {
                ViewModel.GiveFeedback(faq, false);
            }
        }

        private void UpdateAdminVisibility()
        {
            EmployeeActionsPanel.Visibility = ViewModel.IsAdmin
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void ScrollToMiddleIfExpanded(FAQEntryDTO faq)
        {
            if (faq.IsExpanded)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    var container = AllQuestionsList.ContainerFromItem(faq) as FrameworkElement;
                    if (container != null)
                    {
                        container.StartBringIntoView(new BringIntoViewOptions
                        {
                            VerticalAlignmentRatio = 0.5,
                            AnimationDesired = true
                        });
                    }
                });
            }
            
        }

        private void SetCategoryUI(Button selected)
        {
            var normal = (Style)this.Resources["CategoryButtonStyle"];
            var active = (Style)this.Resources["SelectedCategoryButtonStyle"];

            AllQuestionsButton.Style = normal;
            CheckInButton.Style = normal;
            ParkingButton.Style = normal;
            BaggageButton.Style = normal;
            TicketsButton.Style = normal;
            FacilitiesButton.Style = normal;

            selected.Style = active;
        }
    }
}