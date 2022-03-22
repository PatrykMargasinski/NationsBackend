using NationsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Repositories
{
    public interface IPlayerRepository: ICrudRepository<Player> 
    {
        public Player Register(RegisterDTO user);
        public void DeleteAccount(long playerId);
        public object Login(LoginDto user);
        public Player GetByNick(string nick);
        public bool PlayerWithThatNickExist(string nick);
        public void ChangePassword(ChangePasswordDTO changeModel);
        public IList<string> GetNicksStartingWith(string name);
    }
}
