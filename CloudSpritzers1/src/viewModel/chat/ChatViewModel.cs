using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model;
using CloudSpritzers1.Src.Model.Chats;
using CloudSpritzers1.Src.Model.Faq.Bot;
using CloudSpritzers1.Src.Model.Message;
using CloudSpritzers1.Src.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using CloudSpritzers1.Src.Service.Interfaces;

namespace CloudSpritzers1.Src.ViewModel.Chats
{
    public sealed partial class ChatViewModel : ObservableObject
    {
        public ObservableCollection<FAQOption> CurrentOptions { get; } = new ();
        public ObservableCollection<MessageDTO> ChatHistory { get; } = new ();

        private MessageService messageService;
        private ChatService chatService;
        private IUserService userService;
        private IMapper mapper;
        private Chat chat;
        private User user;
        private const int FIRST_OPTION = 1;
        public ChatViewModel(MessageService msgService, ChatService chatService, IMapper mapper, IUserService userService)
        {
            messageService = msgService;
            this.chatService = chatService;
            this.mapper = mapper;
            this.userService = userService;

            // TODO: add null guard
            user = (App.Current as App).User;

            chat = this.chatService.OpenChat(user.RetrieveUniqueDatabaseIdentifierForBot());

            LoadChatHistory();

            if (ChatHistory.Count == 0)
            {
                LoadFirstMessage();
            }
        }

        public string FormatUserId => "User Id: " + user.RetrieveUniqueDatabaseIdentifierForBot().ToString();
        public void CloseChat()
        {
            chatService.CloseChat(chat.ChatId);
        }
        private void LoadChatHistory()
        {
            ChatHistory.Clear();
            var messages = messageService.GetAllMessages(chat.ChatId);
            var currentUserId = user.RetrieveUniqueDatabaseIdentifierForBot();
            foreach (var msg in messages)
            {
                var dto = mapper.Map<MessageDTO>(msg);
                dto.SenderName = userService.GetById(dto.SenderId)?.RetrieveConfiguredDisplayFullNameForBot();
                dto.IsOutgoing = (dto.SenderId == currentUserId);
                ChatHistory.Add(dto);
            }
        }

        [RelayCommand]
        private void HandleOptionClick(FAQOption option)
        {
            if (option == null)
            {
                return;
            }

            BotMessage botReply = messageService.SendMessage(chat.ChatId, user, option);
            System.Diagnostics.Debug.WriteLine($"User selected: {option.label}");

            LoadChatHistory();
            UpdateAvailableOptions(botReply);
        }

        private void UpdateAvailableOptions(BotMessage botReply)
        {
            CurrentOptions.Clear();
            var nextOptions = (botReply as IMessage).GetNextOptions();

            // var dto = _mapper.Map<MessageDTO>();
            if (nextOptions != null)
            {
                foreach (var opt in nextOptions)
                {
                    CurrentOptions.Add(opt);
                }
            }
            else
            {
                CurrentOptions.Add(new FAQOption("Restart Chat", FIRST_OPTION));
            }
        }

        private void LoadFirstMessage()
        {
            HandleOptionClick(new FAQOption("Hello! I need help.", FIRST_OPTION));
            // _messageService.SendMessage(_chat.ChatId, _user, new FAQOption("Hello! I need help.", _FIRST_OPTION));
        }
    }
}
