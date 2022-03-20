using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Utils
{
    public interface ISecurityUtils
    {
        public string Encrypt(string text);
        public string Decrypt(string text);
        public string Hash(string text);
    }
}

