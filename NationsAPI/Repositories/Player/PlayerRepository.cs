using NationsAPI.Models;
using NationsAPI.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using NationsAPI.Utils;
using System;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace NationsAPI.Repositories
{
    
    public class PlayerRepository : CrudRepository<Player>, IPlayerRepository
    {

        private readonly ISecurityUtils _securityUtils;
        private readonly IAuthUtils _authUtils;

        public PlayerRepository(NationsDbContext context,
            ISecurityUtils securityUtils,
            IAuthUtils authUtils
            ) : base(context)
        {
            _securityUtils = securityUtils;
            _authUtils = authUtils;
        }

        public object Login(LoginDto user)
        {
            var errors = LoginVerification(user);
            if (errors.Count > 0)
                throw new Exception(string.Join('\n', errors));
            Player player = GetByNick(user.Nick);
            var token = _authUtils.CreateToken(player);
            return new
            {
                Token = token
            };
        }

        public Player Register(RegisterDTO user)
        {
            var errors = RegisterVerification(user);
            if (errors.Count > 0)
                throw new Exception(string.Join('\n', errors));
            Player player = new Player()
            {
                Nick = user.Nick,
                Password = _securityUtils.Hash(user.Password),
            };
            Create(player);
            return player;
        }

        public void DeleteAccount(long playerId)
        {
            Player deletingPlayer = GetById(playerId);
            if (deletingPlayer == null)
                throw new Exception("There is no player with such id");
            DeleteById(playerId);
        }

        public void ChangePassword(ChangePasswordDTO changeModel)
        {
            Player player = GetById(changeModel.PlayerId);
            if (player == null)
            {
                throw new Exception("User not found");
            }

            if (VerifyPassword(player, changeModel.OldPassword) == false)
            {
                throw new Exception("Invalid old password");
            }
            player.Password = _securityUtils.Hash(changeModel.NewPassword);
            Update(player);
        }

        public Player GetByNick(string nick)
        {
            return _context.Players.FirstOrDefault(player => player.Nick == nick);
        }

        public bool PlayerWithThatNickExist(string nick)
        {
            var playerExist = _context.Players.Any(x => x.Nick == nick);
            return playerExist;
        }

        public IList<string> GetNicksStartingWith(string name)
        {
            var names = _context.Players
                .Where(x =>
                (x.Nick.ToLower()).StartsWith(name))
                .Select(x => x.Nick)
                .Take(5)
                .ToList();
            return names;
        }

        private IList<string> LoginVerification(LoginDto user)
        {
            if (user == null)
            {
                return new string[] { "There is no data" };
            }

            IList<string> errors = new List<string>();
            if (string.IsNullOrEmpty(user.Nick)) errors.Add("Nick is empty");
            if (string.IsNullOrEmpty(user.Password)) errors.Add("Password is empty");
            try
            {
                if (errors.Count > 0)
                {
                    return errors.ToArray();
                }

                Player player =GetByNick(user.Nick);
                if (player == null)
                {
                    return new string[] { "There is no player with such nick" };
                }

                if (VerifyPassword(player, user.Password) == false)
                {
                    return new string[] { "Wrong password" };
                }
                return errors;
            }
            catch (SqlException)
            {
                return new string[] { "There is a problem with a database" };
            }
        }

        private IList<string> RegisterVerification(RegisterDTO model)
        {
            if (model == null)
            {
                return new string[] { "There is no data" };
            }
            IList<string> errors = new List<string>();
            if (model.Nick == "") errors.Add("Nick is empty");
            if (model.Password == "") errors.Add("Password is empty");
            if (PlayerWithThatNickExist(model.Nick) == true)
            {
                errors.Add("There is a player with a such nick");
            }

            return errors;
        }

        private bool VerifyPassword(Player player, string pass)
        {
            string savedPasswordHash = player.Password;
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(pass, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }
    }
}
