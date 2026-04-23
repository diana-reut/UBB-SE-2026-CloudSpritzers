using System;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Employee;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace CloudSpritzers1.Src.ViewModel.General
{
    public sealed partial class UpperBarViewModel : ObservableObject
    {
        private readonly User user;
        private readonly Employee employee;

        public UpperBarViewModel(User? currentUser = null, Employee? currentEmployee = null)
        {
            // Access to application state is performed inside the ViewModel layer
            // user = ((App)App.Current).User;
            // employee = ((App)App.Current).Employee
            user = currentUser ?? (Application.Current is App a ? a.User : null);
            employee = currentEmployee ?? (Application.Current is App ap ? ap.Employee : null);

            // Determine if the current context is a client (user) or an employee
            isClientView = user != null;
        }

        private bool isClientView;
        public bool IsClientView
        {
            get => isClientView;
            private set
            {
                if (SetProperty(ref isClientView, value))
                {
                    // Notify dependent properties
                    OnPropertyChanged(nameof(UserDisplayLabel));
                }
            }
        }

        public string UserDisplayLabel
        {
            get
            {
                if (IsClientView && user != null)
                {
                    return $"ID: {user.RetrieveUniqueDatabaseIdentifierForBot()}";
                }

                if (!IsClientView && employee != null)
                {
                    return $"ID: {employee.RetrieveUniqueDatabaseIdentifierForBot()}";
                }

                return "Not signed in";
            }
        }

        // Navigation targets are resolved here so the view does not contain branching logic
        public Type LandingPageType => typeof(CloudSpritzers1.Src.View.General.LandingPage);
        public Type FAQPageType => typeof(CloudSpritzers1.Src.View.Faq.FAQView);
        public Type ChatPageType => typeof(CloudSpritzers1.Src.View.Chat.ChatPage);
        public Type TicketsPageType => IsClientView
            ? typeof(CloudSpritzers1.Src.View.Ticket.TicketsView)
            : typeof(CloudSpritzers1.Src.View.Ticket.TicketEmployeeView);
        public Type ReviewsPageType => IsClientView
            ? typeof(CloudSpritzers1.Src.View.Review.ReviewPage)
            : typeof(CloudSpritzers1.Src.View.Review.EmployeeSeeReviews);
        public Type ChoosingPageType => typeof(CloudSpritzers1.Src.View.General.ChoosingPage);
    }
}
