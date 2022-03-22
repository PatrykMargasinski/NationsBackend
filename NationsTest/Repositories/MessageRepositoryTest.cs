using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using NationsAPI.Database;
using NationsAPI.Models;
using NationsAPI.Repositories;
using NationsAPI.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NationsTest.Repositories
{
    public class MessageRepositoryTest
    {
        private NationsDbContext _context;
        private IMessageRepository _messageRepository;
        private ISecurityUtils _securityUtils;
        private Player p1;
        private Player p2;
        private Player p3;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<NationsDbContext>().UseInMemoryDatabase("NationsDbTest");
            _context = new NationsDbContext(dbContextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var myConfiguration = new Dictionary<string, string>
            {
                {"Security:EncryptKey", "encryptKey"}
            };

            IConfiguration conf = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _securityUtils = new SecurityUtils(conf);
            _messageRepository = new MessageRepository(_context, _securityUtils);

            p1 = new Player()
            {
                Nick = "p1",
                Password = "test"
            };

            p2 = new Player()
            {
                Nick = "p2",
                Password = "test"
            };

            p3 = new Player()
            {
                Nick = "p3",
                Password = "test"
            };
            _context.Players.Add(p1);
            _context.Players.Add(p2);
            _context.Players.Add(p3);
            _context.SaveChanges();
        }

        [Test]
        public void SendMessageTest()
        {
            var count = _context.Messages.Count();


            var message = new Message()
            { 
                Content = "testContent", 
                Subject = "testSubject", 
                FromPlayerId = p1.Id, 
                ToPlayerId = p2.Id 
            };

            _messageRepository.SendMessage(message);
            Assert.AreEqual(count + 1, _context.Messages.Count());
        }

        [Test]
        public void GetMessageContentTest()
        {
            var message = new Message()
            {
                Content = "testContent",
                Subject = "testSubject",
                FromPlayerId = p1.Id,
                ToPlayerId = p2.Id
            };

            _messageRepository.SendMessage(message);
            var decodedContent = _messageRepository.GetMessageContent(message.Id);
            Assert.IsTrue(decodedContent.Contains("testContent"));
        }

        [Test]
        public void CheckIfSeenStatusIsChangedAfterOpeningMessage()
        {
            var message = new Message()
            {
                Content = "testContent",
                Subject = "testSubject",
                FromPlayerId = p1.Id,
                ToPlayerId = p2.Id,
                Seen = false
            };

            _messageRepository.SendMessage(message);
            _messageRepository.GetMessageContent(message.Id);
            var openedMessage = _messageRepository.GetById(message.Id);
            Assert.IsTrue(openedMessage.Seen == true);
        }

        [Test]
        public void GetAllMessagesToTest()
        {
            var message = new Message()
            {
                Content = "testContent",
                Subject = "testSubject",
                FromPlayerId = p1.Id,
                ToPlayerId = p2.Id
            };
            _messageRepository.SendMessage(message);
            var messages = _messageRepository.GetAllMessagesTo(p2.Id);
            Assert.AreEqual(1, messages.Count);
        }

        [Test]
        public void GetAllMessagesCheckIfFilteringByPlayersNickWorksProperly()
        {
            var m1 = new Message()
            {
                Content = "testContent",
                Subject = "testSubject",
                FromPlayerId = p1.Id,
                ToPlayerId = p2.Id
            };

            var m2 = new Message()
            {
                Content = "testContent",
                Subject = "testSubject",
                FromPlayerId = p3.Id,
                ToPlayerId = p2.Id
            };
            _messageRepository.SendMessage(m1);
            _messageRepository.SendMessage(m2);
            var messages = _messageRepository.GetAllMessagesTo(p2.Id, playerNameFilter: "p3");
            var foundP1 = messages.FirstOrDefault(x => x.FromPlayer == "p1");
            var foundP3 = messages.FirstOrDefault(x => x.FromPlayer == "p3");
            Assert.IsNull(foundP1);
            Assert.NotNull(foundP3);
        }

        [Test]
        public void GetAllMessagesCheckIfPagingWorksProperly()
        {
            for(int i=0;i<10;i++)
            {
                var message= new Message()
                {
                    Content = $"testContent{i}",
                    Subject = "testSubject",
                    FromPlayerId = p1.Id,
                    ToPlayerId = p2.Id
                };
                _messageRepository.SendMessage(message);
            }
            var messages = _messageRepository.GetAllMessagesTo(p2.Id, fromRange: 3, toRange: 6);
            Assert.AreEqual(3, messages.Count);
            var firstMessageContent = _messageRepository.GetMessageContent(messages[0].Id);
            Assert.IsTrue(firstMessageContent.Contains("testContent6"));
            var lastMessageContent = _messageRepository.GetMessageContent(messages[^1].Id);
            Assert.IsTrue(lastMessageContent.Contains("testContent4"));
        }

        [Test]
        public void GetAllMessagesCheckIfOnlyUnseenFilteringWorksProperly()
        {
            for (int i = 0; i < 5; i++)
            {
                var message = new Message()
                {
                    Content = $"testContent{i}",
                    Subject = "testSubject",
                    FromPlayerId = p1.Id,
                    ToPlayerId = p2.Id,
                    Seen = true
                };
                _messageRepository.SendMessage(message);
            }

            for (int i = 0; i < 5; i++)
            {
                var message = new Message()
                {
                    Content = $"testUnseenContent{i}",
                    Subject = "testSubject",
                    FromPlayerId = p1.Id,
                    ToPlayerId = p2.Id,
                    Seen = false
                };
                _messageRepository.SendMessage(message);
            }
            var messages = _messageRepository.GetAllMessagesTo(p2.Id, onlyUnseen: true);
            Assert.AreEqual(5, messages.Count);
            foreach(var message in messages)
            {
                var content = _messageRepository.GetMessageContent(message.Id);
                Assert.IsTrue(content.Contains("Unseen"));
            }
        }
    }
}