using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KindAds.Negocio.Utilerias
{
    public class StringUtilities
    {
        public static string RemoveNewLineInString(string candidate)
        {
            return Regex.Replace(candidate, @"\t|\n|\r", "");
        }
    }
}
