using System;
using System.Collections.ObjectModel;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Service;
using CloudSpritzers1.Src.Service.Bot;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CloudSpritzers1.Src.Service.Interfaces;

namespace CloudSpritzers1.Src.ViewModel
{
    public partial class MessageViewModel : ObservableObject
    {
        private readonly MessageService messageService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        private readonly int chatId;
        private readonly int currentUserId;

        public ObservableCollection<MessageDTO> Messages { get; } = new ();

        public MessageViewModel(
            MessageService messageService,
            IUserService userService,
            IMapper mapper,
            int chatId,
            int currentUserId)
        {
            this.messageService = messageService;
            this.userService = userService;
            this.mapper = mapper;
            this.chatId = chatId;
            this.currentUserId = currentUserId;

            LoadMessages();
        }

        public void LoadMessages()
        {
            var messagesFromDb = messageService.GetAllMessages(chatId);
            Messages.Clear();

            foreach (var message in messagesFromDb)
            {
                Messages.Add(mapper.Map<MessageDTO>(message));
            }
        }

        [RelayCommand]
        public void SendMessage(FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }

            // Lazily resolve the current user only when needed.
            var sender = userService.GetById(currentUserId);

            BotMessage botReply = messageService.SendMessage(chatId, sender, selectedOption);

            Messages.Add(mapper.Map<MessageDTO>(new Message(sender, botReply.GetChat(), selectedOption.label)));
            Messages.Add(mapper.Map<MessageDTO>(botReply));
        }
    }
}