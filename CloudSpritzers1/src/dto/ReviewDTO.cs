using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.src.dto
{
    public record ReviewDTO(
        int reviewId,
        int userId,
        string userName,
        string message,
        int dutyFreeRating,
        int flightExperienceRating,
        int staffFriendlinessRating,
        int cleanlinessRating,
        float overallRating
    );
}
