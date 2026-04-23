using System;
using CloudSpritzers1.Src.ViewModel;
using CloudSpritzers1.Src.ViewModel.Chats;
using CloudSpritzers1.Src.ViewModel.General;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.Src.View.Chat
{
    public sealed partial class ChatPage : Page
    {
        public ChatViewModel ViewModel { get; }
        public ChatPage()
        {
            ViewModel = (App.Current as App).Services.GetService<ChatViewModel>();
            this.InitializeComponent();
        }

        public void EndChat(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseChat();
            this.Frame.Navigate(typeof(CloudSpritzers1.Src.View.General.LandingPage));
        }
    }
}