using System;

namespace CloudSpritzers1.src.DTO
{
    public record MessageDTO(
        int MessageId,
        int ChatId,
        int SenderId,       // numeric FK matching the DB column sender_id
        string MessageText,
        DateTimeOffset Timestamp
    );
}
