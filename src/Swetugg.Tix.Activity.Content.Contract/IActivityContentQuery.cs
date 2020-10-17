using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swetugg.Tix.Activity.Content.Contract
{
    public interface IActivityContentQuery
    {
        Task<ActivityContent> GetActivity(Guid activityId);
        Task<IEnumerable<TicketTypeContent>> GetTicketTypes(Guid activityId);
    }
}
