using AutoMapper;
using CloudSpritzers1.src.dto;
using CloudSpritzers1.src.model;
using CloudSpritzers1.src.model.chat;
using CloudSpritzers1.src.model.faq.bot;
using CloudSpritzers1.src.model.message;
using CloudSpritzers1.src.service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.service.interfaces;

namespace CloudSpritzers1.src.viewModel.chat
{
    public sealed partial class ChatViewModel : ObservableObject
    {
        public ObservableCollection<FAQOption> CurrentOptions { get; } = new();
        public ObservableCollection<MessageDTO> ChatHistory { get; } = new();

        private MessageService _messageService;
        private ChatService _chatService;
        private IUserService _userService;
        private IMapper _mapper;
        private Chat _chat;
        private User _user;
        private const int _FIRST_OPTION = 1;


        public ChatViewModel(MessageService msgService,ChatService chatService, IMapper mapper, IUserService userService) {
            _messageService = msgService;
            _chatService = chatService;
            _mapper = mapper;
            _userService = userService;

            // TODO: add null guard
            _user = (App.Current as App).User; 

            _chat = _chatService.OpenChat(_user.RetrieveUniqueDatabaseIdentifierForBot());

            LoadChatHistory();

            if (ChatHistory.Count == 0)
            {
                LoadFirstMessage();
            }

        }

        public string FormatUserId => "User Id: " + _user.RetrieveUniqueDatabaseIdentifierForBot().ToString();
        
        public void CloseChat()
        {
            _chatService.CloseChat(_chat.ChatId);
        }


        private void LoadChatHistory()
        {
            ChatHistory.Clear();
            var messages = _messageService.GetAllMessages(_chat.ChatId);
            var currentUserId = _user.RetrieveUniqueDatabaseIdentifierForBot();
            foreach (var msg in messages)
            {
                var dto = _mapper.Map<MessageDTO>(msg);
                dto.SenderName = _userService.GetById(dto.SenderId)?.RetrieveConfiguredDisplayFullNameForBot();
                dto.IsOutgoing = (dto.SenderId == currentUserId);
                ChatHistory.Add(dto);
            }
        }

        [RelayCommand]
        private void HandleOptionClick(FAQOption option)
        {
            if (option == null) return;


            BotMessage botReply = _messageService.SendMessage(_chat.ChatId, _user, option);
            System.Diagnostics.Debug.WriteLine($"User selected: {option.Label}");

            LoadChatHistory() ;
            UpdateAvailableOptions(botReply);

        }

        private void UpdateAvailableOptions(BotMessage botReply)
        {
            CurrentOptions.Clear();
            var nextOptions = (botReply as IMessage).GetNextOptions();
            //var dto = _mapper.Map<MessageDTO>();

            if (nextOptions != null)
            {
                foreach (var opt in nextOptions)
                {
                    CurrentOptions.Add(opt); 
                }
            }
            else
            {
                CurrentOptions.Add(new FAQOption("Restart Chat", _FIRST_OPTION));
            }
        }

        private void LoadFirstMessage()
        {
            HandleOptionClick(new FAQOption("Hello! I need help.", _FIRST_OPTION));
            //_messageService.SendMessage(_chat.ChatId, _user, new FAQOption("Hello! I need help.", _FIRST_OPTION));
        }

    }
}
