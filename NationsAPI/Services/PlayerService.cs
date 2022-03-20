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

        public void CreateUser(RegisterDTO user)
        {
            _playerRepository.CreateUser(user);
        }

        public void DeleteAccount(long playerId)
        {
            _playerRepository.DeleteAccount(playerId);
        }

        public void ChangePassword(ChangePasswordDTO changeModel)
        {
            _playerRepository.ChangePassword(changeModel);
        }

        public IList<string> GetSimilarNames(string name)
        {
            var similarNames = _playerRepository.GetSimilarNames(name.ToLower().Replace(" ", ""));
            return similarNames;
        }

        public long GetIdByNick(string nick)
        {
            var player = _playerRepository.GetByNick(nick);
            return player.Id;
        }
    }
}
