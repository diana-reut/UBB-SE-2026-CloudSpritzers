using System;
using CloudSpritzers1.Src.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.Src.View.General
{
    public sealed partial class LandingPage : Page
    {
        private DispatcherTimer carouselTimer = new DispatcherTimer();

        public LandingViewModel ViewModel { get; }

        public LandingPage()
        {
            ViewModel = (App.Current as App).Services.GetService<LandingViewModel>();
            this.InitializeComponent();
            this.DataContext = ViewModel;
            StartCarousel();
            this.Loaded += (s, e) => ViewModel.LoadReviews();
        }

        private void StartCarousel()
        {
            carouselTimer.Interval = TimeSpan.FromSeconds(2);
            carouselTimer.Tick += OnCarouselTick;
            carouselTimer.Start();
        }

        private void OnCarouselTick(object? sender, object e)
        {
            if (SupportCarousel.Items.Count <= 1)
            {
                return;
            }

            int nextIndex = (SupportCarousel.SelectedIndex + 1) % SupportCarousel.Items.Count;
            SupportCarousel.SelectedIndex = nextIndex;
        }
    }
}