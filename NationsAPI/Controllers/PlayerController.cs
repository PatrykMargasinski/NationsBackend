using NationsAPI.Models;
using NationsAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationsAPI.Services;

namespace NationsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : Controller
    {
        private readonly PlayerService _playerService;
        public PlayerController(PlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginDto user)
        {
            try
            {
                var token = _playerService.Login(user);
                return Ok(token);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterDTO user)
        {
            try
            {
                _playerService.Register(user);
                return Ok();
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("/changePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDTO changeModel)
        {
            try
            {
                _playerService.ChangePassword(changeModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("/deleteAccount/{playerId:long}")]
        public IActionResult DeleteAccount(long playerId)
        {
            try
            {
                _playerService.DeleteAccount(playerId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("similarNames")]
        public IActionResult GetNicksStartingWith(string name)
        {
            var similarNames = _playerService.GetNicksStartingWith(name);
            return new JsonResult(similarNames);
        }

        [HttpGet("{nick}/id")]
        public IActionResult GetIdByNick(string nick)
        {
            long id = _playerService.GetIdByNick(nick);
            return new JsonResult(id);
        }
    }
}
