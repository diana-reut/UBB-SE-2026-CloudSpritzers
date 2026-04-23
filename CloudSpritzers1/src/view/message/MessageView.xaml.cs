using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.viewmodel;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.src.view.message
{
    public sealed partial class MessageView : UserControl
    {
        public MessageDTO DataTransferObjectContainingMessageDetailsForViewModelBinding => (MessageDTO)DataContext;

        public MessageView()
        {
            this.InitializeComponent();
            this.DataContextChanged += (senderObjectTriggeringEvent, eventArgumentsContainingDataContextInformation) =>
            {
                if (eventArgumentsContainingDataContextInformation.NewValue is MessageDTO)
                {
                    this.Bindings.Update();
                }
            };

        }
    }
}