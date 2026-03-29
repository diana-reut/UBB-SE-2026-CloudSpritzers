using System;

namespace CloudSpritzers1.src.DTO
{
    public record MessageDTO(
        int MessageId,
        int ChatId,
        string SenderName,
        string MessageText,
        bool IsRead, //FIXME: Should I add it?
        DateTimeOffset Timestamp
    );
}