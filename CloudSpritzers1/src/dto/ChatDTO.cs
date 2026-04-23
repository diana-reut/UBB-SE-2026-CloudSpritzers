using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Model.Chats;

namespace CloudSpritzers1.Src.Dto
{
    public record ChatDTO(int chatId, int userId, ChatStatus status, int messageCount);
}
