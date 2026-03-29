using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CloudSpritzers1.src.view.message
{
    public sealed partial class MessageView : UserControl
    {
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register(
                nameof(MessageText), typeof(string), typeof(MessageView),
                new PropertyMetadata(string.Empty, OnMessageTextChanged));

        public static readonly DependencyProperty SenderNameProperty =
            DependencyProperty.Register(
                nameof(SenderName), typeof(string), typeof(MessageView),
                new PropertyMetadata(string.Empty, OnSenderNameChanged));

        public static readonly DependencyProperty TimestampProperty =
            DependencyProperty.Register(
                nameof(Timestamp), typeof(DateTimeOffset), typeof(MessageView),
                new PropertyMetadata(DateTimeOffset.MinValue, OnTimestampChanged));

        public static readonly DependencyProperty IsOutgoingProperty =
            DependencyProperty.Register(
                nameof(IsOutgoing), typeof(bool), typeof(MessageView),
                new PropertyMetadata(false, OnIsOutgoingChanged));

        public string MessageText
        {
            get => (string)GetValue(MessageTextProperty);
            set => SetValue(MessageTextProperty, value);
        }

        public string SenderName
        {
            get => (string)GetValue(SenderNameProperty);
            set => SetValue(SenderNameProperty, value);
        }

        public DateTimeOffset Timestamp
        {
            get => (DateTimeOffset)GetValue(TimestampProperty);
            set => SetValue(TimestampProperty, value);
        }

        public bool IsOutgoing
        {
            get => (bool)GetValue(IsOutgoingProperty);
            set => SetValue(IsOutgoingProperty, value);
        }

        public MessageView()
        {
            this.InitializeComponent();
        }

        private static void OnMessageTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageView)d).MessageTextBlock.Text = (string)e.NewValue;
        }

        private static void OnSenderNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageView)d).SenderLabel.Text = (string)e.NewValue;
        }

        private static void OnTimestampChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (MessageView)d;
            var ts = (DateTimeOffset)e.NewValue;
            view.TimestampLabel.Text = ts == DateTimeOffset.MinValue
                ? string.Empty
                : ts.ToLocalTime().ToString("HH:mm");
        }

        private static void OnIsOutgoingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (MessageView)d;
            bool outgoing = (bool)e.NewValue;

            view.BubbleBorder.HorizontalAlignment =
                outgoing ? HorizontalAlignment.Right : HorizontalAlignment.Left;

            view.SenderLabel.Visibility =
                outgoing ? Visibility.Collapsed : Visibility.Visible;

            if (outgoing)
            {
                view.BubbleBorder.Background = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 58, 176, 176));
                view.MessageTextBlock.Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255,255,255,255));
            }
            else
            {
                view.BubbleBorder.Background = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 255, 255, 255));
                view.MessageTextBlock.Foreground = new SolidColorBrush(
                    Windows.UI.Color.FromArgb(255, 0, 0, 0));
            }
        }
    }
}
