using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NationsAPI.Util
{
    public static class Utils
    {
        public static bool IsAlphabets(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return false;

            for (int i = 0; i < inputString.Length; i++)
                if (!char.IsLetter(inputString[i]))
                    return false;
            return true;
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
