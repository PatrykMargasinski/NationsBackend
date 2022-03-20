using NationsAPI.Models;
using System.Linq;
using NationsAPI.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NationsAPI.Utils;
using System;

namespace NationsAPI.Repositories
{

    public class MessageRepository : CrudRepository<Message>, IMessageRepository
    {

        private readonly ISecurityUtils _securityUtils;
        public MessageRepository(NationsDbContext context, ISecurityUtils securityUtils) : base(context)
        {
            _securityUtils = securityUtils;
        }

        public IList<MessageDTO> GetAllMessagesTo(long PlayerId, int? fromRange = 0, int? toRange = 5, string playerNameFilter = "", bool onlyUnseen = false)
        {
            return _context.Messages
                .Include(x => x.ToPlayer)
                .Include(x => x.FromPlayer)
                .Where(mes =>
                mes.ToPlayerId == PlayerId && (
                    (mes.FromPlayer.Nick).ToLower().Contains(playerNameFilter.Trim().ToLower())
                ) &&
                    (!mes.Seen || !onlyUnseen) //get only unseen messages if "onlyUnseen" is true
                )
                .OrderByDescending(x => x.ReceivedDate)
                .Skip(fromRange.Value)
                .Take(toRange.Value - fromRange.Value)
                .Select(x => new MessageDTO
                {
                    Id = x.Id,
                    FromPlayer = x.FromPlayer.Nick,
                    ToPlayer = x.ToPlayer.Nick,
                    Subject = _securityUtils.Decrypt(x.Subject),
                    ReceivedDate = x.ReceivedDate,
                    Seen = x.Seen
                }
                )
                .ToList();
        }


        public int CountMessagesTo(long playerId)
        {
            return _context.Messages
                .Where(x => x.ToPlayerId == playerId)
                .Count();
        }

        public void SendMessage(Message message)
        {
            message.Content = _securityUtils.Encrypt(message.Content);
            message.Subject = _securityUtils.Encrypt(message.Subject);
            message.ReceivedDate = DateTime.Now;
            Create(message);
        }

        public string GetMessageContent(long id)
        {
            var message = GetById(id);
            message.Seen = true;
            Update(message);
            var content = _securityUtils.Decrypt(message.Subject) + "\n\n" +
                _securityUtils.Decrypt(message.Content);
            return content;
        }
    }
}
