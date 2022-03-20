using NationsAPI.Models;
using NationsAPI.Repositories;
using NationsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NationsAPI.Services.Messages;

namespace NationsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("to")]
        public JsonResult GetAllMessagesTo(long playerId, int? fromRange, int? toRange, string playerNameFilter = "", bool onlyUnseen = false)
        {
            var messages = _messageService.GetAllMessagesTo(playerId, fromRange.Value, toRange.Value, playerNameFilter, onlyUnseen);
            return new JsonResult(messages);
        }

        [HttpGet("count")]
        public JsonResult CountMessagesTo(long playerId)
        {
            var messageCount = _messageService.CountMessagesTo(playerId);
            return new JsonResult(messageCount);
        }

        [HttpPost]
        public JsonResult SendMessage(Message message)
        {
            _messageService.SendMessage(message);
            return new JsonResult("Added successfully");
        }

        [HttpGet("content")]
        public JsonResult GetMessageContent(long id)
        {
            var content = _messageService.GetMessageContent(id);
            return new JsonResult(content);
        }

        [HttpDelete("{id}")]
        public JsonResult DeleteMessage(long id)
        {
            _messageService.DeleteMessage(id);
            return new JsonResult("Deleted successfully");
        }

        [Route("/messages")]
        [HttpDelete]
        public JsonResult DeleteMessages(string stringIds)
        {
            _messageService.DeleteMessages(stringIds);
            return new JsonResult("Deleted successfully");
        }
    }
}
