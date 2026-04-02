using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.employee;
using CloudSpritzers1.src.viewModel.general;
using CloudSpritzers1.src.viewModel.review;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.src.view.general
{
    public sealed partial class UpperBar : UserControl
    {

        public UpperBarViewModel ViewModel { get; }

        private User _user;
        private Employee _employee;

        public UpperBar()
        {
            this.InitializeComponent();

            _user = ((App)App.Current).User;
            _employee = ((App)App.Current).Employee;

            ViewModel = (App.Current as App).Services.GetService<UpperBarViewModel>();
            this.DataContext = ViewModel;
            ViewModel.IsEmployee = !(App.Current as App).isEmployee;
        }

        private DependencyObject FindParentFrame()
        {
            DependencyObject parent = this.Parent;
            while (parent != null && !(parent is Frame))
            {
                parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        public void NavigateToChat(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(typeof(CloudSpritzers1.src.view.chat.ChatPage));
            }
        }

        public void NavigateToLanding(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(typeof(CloudSpritzers1.src.view.general.LandingPage));
            }
        }

        public void NavigateToFAQ(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(typeof(CloudSpritzers1.src.view.faq.FAQView));
            }
        }

        public void NavigateToTickets(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(typeof(CloudSpritzers1.src.view.TicketsView));
            }
        }

        public void NavigateToReviews(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                if(_user != null)
                    frame.Navigate(typeof(CloudSpritzers1.src.view.review.ReviewPage));
                else
                    frame.Navigate(typeof(CloudSpritzers1.src.view.review.EmployeeSeeReviews));
            }
        }
    }
}