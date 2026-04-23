using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace CloudSpritzers1.Src.View.Message
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