
using NationsAPI.Models;
using NationsAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NationsAPI.Services.Messages
{

    public class MessageService
    {
        private readonly IMessageRepository _messageRepository;
        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public IList<MessageDTO> GetAllMessagesTo(long playerId, int? fromRange, int? toRange, string playerNameFilter = "", bool onlyUnseen = false)
        {
            var messages = _messageRepository
                .GetAllMessagesTo(playerId, fromRange, toRange, playerNameFilter, onlyUnseen);
            return messages.ToList();

        }
        public void SendMessage(Message message)
        {
            _messageRepository.SendMessage(message);
        }

        public string GetMessageContent(long id)
        {
            var content = _messageRepository.GetMessageContent(id);
            return content;
        }

        public void DeleteMessage(long id)
        {
            _messageRepository.DeleteById(id);
        }

        public void DeleteMessages(string stringIds)
        {
            long[] ids = stringIds.Split('i').Select(x => long.Parse(x)).ToArray();
            _messageRepository.DeleteByIds(ids);
        }

        public int CountMessagesTo(long playerId)
        {
            return _messageRepository.CountMessagesTo(playerId);
        }
    }
}
