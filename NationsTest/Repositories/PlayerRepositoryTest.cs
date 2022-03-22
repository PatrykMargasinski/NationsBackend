using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NationsAPI.Database;
using NationsAPI.Models;
using NationsAPI.Repositories;
using NationsAPI.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationsTest.Repositories
{
    public class PlayerRepositoryTest
    {
        private NationsDbContext _context;
        private IPlayerRepository _playerRepository;

        [SetUp]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<NationsDbContext>().UseInMemoryDatabase("NationsDbTest");
            _context = new NationsDbContext(dbContextOptions.Options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var myConfiguration = new Dictionary<string, string>
            {
                {"Security:AuthKey", "veryVerysecretAuthKey"}
            };

            IConfiguration conf = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            _playerRepository = new PlayerRepository(_context, new SecurityUtils(conf), new AuthUtils(conf));
        }

        [Test]
        public void RegisterTest()
        {
            var datas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };

            _playerRepository.Register(datas);
            var foundUser = _context.Players.FirstOrDefault(x => x.Nick == "test");
            Assert.NotNull(foundUser);
            Assert.AreNotEqual("test", foundUser.Password);
        }

        [Test]
        public void LoginTestCheckIfCorrectPassword()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Register(registerDatas);

            var datas = new LoginDto()
            {
                Nick = "test",
                Password = "test"
            };
            var token = _playerRepository.Login(datas);
            Assert.NotNull(token);
        }

        [Test]
        public void LoginTestCheckIfIncorrectPassword()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Register(registerDatas);

            var datas = new LoginDto()
            {
                Nick = "test",
                Password = "test2"
            };
            var exception = Assert.Throws<Exception>(() => _playerRepository.Login(datas));
            Assert.IsTrue(exception.Message.Contains("Wrong password"));
        }

        [Test]
        public void GetByNickTest()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Register(registerDatas);
            var player = _playerRepository.GetByNick("test");
            Assert.NotNull(player);
        }

        [Test]
        public void PlayerWithThatNickExistTest()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };
            _playerRepository.Register(registerDatas);
            var isPlayer = _playerRepository.PlayerWithThatNickExist("test");
            Assert.NotNull(isPlayer);
        }

        [Test]
        public void ChangePasswordTest()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };

            var player = _playerRepository.Register(registerDatas);
            Assert.NotNull(player);

            var changePasswordDatas = new ChangePasswordDTO()
            {
                NewPassword = "test2",
                OldPassword = "test",
                PlayerId = player.Id
            };
            _playerRepository.ChangePassword(changePasswordDatas);

            var loginDatas = new LoginDto()
            {
                Nick = "test",
                Password = "test2"
            };
            Assert.NotNull(_playerRepository.Login(loginDatas));
        }

        [Test]
        public void ChangePasswordTestCheckIfResponseIsCorrectWhenOldPasswordIsIncorrect()
        {
            var registerDatas = new RegisterDTO()
            {
                Nick = "test",
                Password = "test"
            };

            var player = _playerRepository.Register(registerDatas);
            Assert.NotNull(player);

            var changePasswordDatas = new ChangePasswordDTO()
            {
                NewPassword = "test2",
                OldPassword = "test2",
                PlayerId = player.Id
            };

            var exception = Assert.Throws<Exception>(() => _playerRepository.ChangePassword(changePasswordDatas));
            Assert.IsTrue(exception.Message.Contains("Invalid old password"));
        }

        [Test]
        public void Get5NicksStartingWithTest()
        {
            for(int i=0;i<10;i++)
            {
                var registerDatas = new RegisterDTO()
                {
                    Nick = (i % 2 == 0 ? $"player{i}" : $"coolerPlayer{i}"),
                    Password = "test"
                };
                _playerRepository.Register(registerDatas);
            }
            var playerNicks = _playerRepository.GetNicksStartingWith("cooler");
            Assert.AreEqual(5, playerNicks.Count);
            foreach(var nick in playerNicks)
            {
                Assert.IsTrue(nick.StartsWith("cooler"));
            }
        }
    }
}
