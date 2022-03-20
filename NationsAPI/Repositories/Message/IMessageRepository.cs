using NationsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Repositories
{

    public interface IMessageRepository : ICrudRepository<Message>
    {
        public IList<MessageDTO> GetAllMessagesTo(long playerId, int? fromRange = 0, int? toRange = 5, string playerNameFilter = "", bool onlyUnseen = false);
        public int CountMessagesTo(long playerId);
        public void SendMessage(Message message);
        public string GetMessageContent(long id);
    }
}
