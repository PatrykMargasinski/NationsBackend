using NationsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Repositories
{

    public interface IMessageRepository : ICrudRepository<Message>
    {
        public IList<MessageDTO> GetAllMessagesTo(long playerId, int? fromRange, int? toRange, string PlayerNameFilter, bool onlyUnseen);
        public int CountMessagesTo(long playerId);
        public void SendMessage(Message message);
        public string GetMessageContent(long id);
    }
}
