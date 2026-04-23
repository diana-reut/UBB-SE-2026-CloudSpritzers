using CloudSpritzers1.src.viewmodel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace CloudSpritzers1.src.view.general
{
    public sealed partial class LandingPage : Page
    {
        private DispatcherTimer _carouselTimer = new DispatcherTimer();

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
            _carouselTimer.Interval = TimeSpan.FromSeconds(2);
            _carouselTimer.Tick += OnCarouselTick;
            _carouselTimer.Start();
        }

        private void OnCarouselTick(object? sender, object e)
        {
            if (SupportCarousel.Items.Count <= 1) return;

            int nextIndex = (SupportCarousel.SelectedIndex + 1) % SupportCarousel.Items.Count;
            SupportCarousel.SelectedIndex = nextIndex;
        }
    }
}