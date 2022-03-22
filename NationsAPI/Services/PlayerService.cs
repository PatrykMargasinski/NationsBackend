using NationsAPI.Models;
using NationsAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Services
{
    public class PlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        public PlayerService(
            IPlayerRepository playerRepository
        )
        {
            _playerRepository = playerRepository;
        }

        public object Login(LoginDto user)
        {
            return _playerRepository.Login(user);
        }

        public Player Register(RegisterDTO user)
        {
            return _playerRepository.Register(user);
        }

        public void DeleteAccount(long playerId)
        {
            _playerRepository.DeleteAccount(playerId);
        }

        public void ChangePassword(ChangePasswordDTO changeModel)
        {
            _playerRepository.ChangePassword(changeModel);
        }

        public IList<string> GetNicksStartingWith(string name)
        {
            var similarNames = _playerRepository.GetNicksStartingWith(name.ToLower().Replace(" ", ""));
            return similarNames;
        }

        public long GetIdByNick(string nick)
        {
            var player = _playerRepository.GetByNick(nick);
            return player.Id;
        }
    }
}
