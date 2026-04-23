using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Repository;
using CloudSpritzers1.Src.Repository.Database;
using CloudSpritzers1.Src.Model.Chats;

namespace CloudSpritzers1.Src.Service
{
    public class ChatService
    {
        private IRepository<int, Chat> chatRepository;

        public ChatService(IRepository<int, Chat> chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public Chat OpenChat(int userId)
        {
            try
            {
                Chat newChat = new Chat(0, userId, ChatStatus.Active);
                int newId = Convert.ToInt32(chatRepository.CreateNewEntity(newChat));
                newChat.ChatId = newId;
                return newChat;
            }
            catch (Exception ex)
            {
                    throw (new Exception(message: ex.Message));
            }
        }

        public void CloseChat(int chatId)
        {
            try
            {
                Chat chat = chatRepository.GetById(chatId);
                chat.CloseChat();
                chatRepository.UpdateById(chatId, chat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
