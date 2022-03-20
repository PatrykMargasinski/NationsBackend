using NationsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Repositories
{
    public interface IPlayerRepository: ICrudRepository<Player> 
    {
        public void CreateUser(RegisterDTO user);
        public void DeleteAccount(long playerId);
        public object Login(LoginDto user);
        public Player GetByNick(string nick);
        public bool IsPlayerWithThatNick(string nick);
        public bool VerifyPassword(Player player, string pass);
        public void ChangePassword(ChangePasswordDTO changeModel);
        public IList<string> GetSimilarNames(string name);
    }
}
