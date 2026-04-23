using CloudSpritzers1.Src.ViewModel.General;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.Src.View.General
{
    public sealed partial class UpperBar : UserControl
    {
        public CloudSpritzers1.Src.ViewModel.General.UpperBarViewModel ViewModel { get; }

        public UpperBar()
        {
            this.InitializeComponent();

            ViewModel = (App.Current as App).Services.GetService<CloudSpritzers1.Src.ViewModel.General.UpperBarViewModel>();
            this.DataContext = ViewModel;
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

        public void OnChatRequested(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.ChatPageType);
            }
        }

        public void OnLandingRequested(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.LandingPageType);
            }
        }

        public void OnFAQRequested(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.FAQPageType);
            }
        }

        public void OnTicketsRequested(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.TicketsPageType);
            }
        }

        public void OnReviewsRequested(object sender, RoutedEventArgs e)
        {
            if (FindParentFrame() is Frame frame)
            {
                frame.Navigate(ViewModel.ReviewsPageType);
            }
        }

        public void OnHomeRequested(object sender, RoutedEventArgs e)
        {
            // Navigate to choosing page using the Window's root content safe for WinUI 3
            if (this.XamlRoot.Content is Frame rootFrame)
            {
                rootFrame.Navigate(ViewModel.ChoosingPageType);
            }
        }
    }
}