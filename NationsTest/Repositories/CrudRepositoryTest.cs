using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using NationsAPI.Database;
using NationsAPI.Models;
using NationsAPI.Repositories;
using NationsAPI.Utils;
using NUnit.Framework;
using System;

namespace NationsTest.Repositories
{
    public class CrudRepositoryTest
    {
        private NationsDbContext _context;
        private ICrudRepository<Player> _playerRepository;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<NationsDbContext>().UseInMemoryDatabase("NationsDbTest");
            _context = new NationsDbContext(dbContextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            IConfiguration conf = new ConfigurationBuilder().Build();

            _playerRepository = new PlayerRepository(_context, new SecurityUtils(conf), new AuthUtils(conf));
        }

        [Test]
        public void CreateTest()
        {
            var count = _context.Players.Count();
            var player = new Player()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Create(player);
            Assert.AreEqual(count + 1, _context.Players.Count());
        }

        [Test]
        public void GetByIdTest()
        {
            var player = new Player()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Create(player);
            var id = player.Id;
            var foundPlayer = _playerRepository.GetById(id);
            Assert.NotNull(foundPlayer);
        }

        [Test]
        public void GetAllTest()
        {
            var p1 = new Player()
            {
                Nick = "test1",
                Password = "test"
            };
            var p2 = new Player()
            {
                Nick = "test2",
                Password = "test"
            };
            var p3 = new Player()
            {
                Nick = "test3",
                Password = "test"
            };
            _playerRepository.Create(p1);
            _playerRepository.Create(p2);
            _playerRepository.Create(p3);
            var players = _playerRepository.GetAll();
            Assert.NotNull(players.FirstOrDefault(x => x.Nick == "test1"));
            Assert.NotNull(players.FirstOrDefault(x => x.Nick == "test2"));
            Assert.NotNull(players.FirstOrDefault(x => x.Nick == "test3"));
        }

        [Test]
        public void UpdateTest()
        {
            var player = new Player()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Create(player);
            player.Nick = "updateTest";
            _playerRepository.Update(player);
            var foundPlayer = _playerRepository.GetById(player.Id);
            Assert.AreEqual("updateTest", foundPlayer.Nick);
        }

        [Test]
        public void DeleteTest()
        {
            var count = _context.Players.Count();
            var player = new Player()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Create(player);
            Assert.AreEqual(count + 1, _context.Players.Count());
            _playerRepository.Delete(player);
            Assert.AreEqual(count, _context.Players.Count());
            Assert.IsNull(_context.Players.FirstOrDefault(x=>x.Id==player.Id));
        }

        [Test]
        public void DeleteByIdTest()
        {
            var count = _context.Players.Count();
            var player = new Player()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Create(player);
            Assert.AreEqual(count + 1, _context.Players.Count());
            _playerRepository.DeleteById(player.Id);
            Assert.AreEqual(count, _context.Players.Count());
            Assert.IsNull(_context.Players.FirstOrDefault(x => x.Id == player.Id));
        }

        [Test]
        public void DeleteByIdsTest()
        {
            var count = _context.Players.Count();
            var p1 = new Player()
            {
                Nick = "t1",
                Password = "test"
            };
            var p2 = new Player()
            {
                Nick = "t2",
                Password = "test"
            };
            _playerRepository.Create(p1);
            _playerRepository.Create(p2);
            Assert.AreEqual(count + 2, _context.Players.Count());
            _playerRepository.DeleteByIds(new long[] { p1.Id, p2.Id });
            Assert.AreEqual(count, _context.Players.Count());
            Assert.IsNull(_context.Players.FirstOrDefault(x => x.Id == p1.Id));
            Assert.IsNull(_context.Players.FirstOrDefault(x => x.Id == p2.Id));
        }
    }
}