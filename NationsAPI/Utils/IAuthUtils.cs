using NationsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NationsAPI.Utils
{
    public interface IAuthUtils
    {
        public string CreateToken(Player player);
    }
}
