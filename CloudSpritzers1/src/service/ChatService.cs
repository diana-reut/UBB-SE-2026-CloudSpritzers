using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.src.repository;
using CloudSpritzers1.src.repository.database;
using CloudSpritzers1.src.model.chat;

namespace CloudSpritzers1.src.service
{
    public class ChatService
    {
        private IRepository<int, Chat> _repository;

        public ChatService(ChatDatabaseRepository repository)
        {
            _repository = repository;
        }

        public Chat OpenChat(int userId)
        {
            try
            {
                Chat newChat = new Chat(0, userId, ChatStatus.Active);
                int newId = Convert.ToInt32(_repository.CreateNewEntity(newChat));
                newChat.ChatId = newId;
                return newChat;

            }
            catch(Exception ex)
            {
                    throw(new Exception(message:  ex.Message));
            }
        }

        public void CloseChat(int chatId)
        {
            try
            {
                Chat chat = _repository.GetById(chatId);
                chat.CloseChat();
                _repository.UpdateById(chatId, chat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
