using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTMUtil
{
    public static class DTMPath
    {
        public static string GetDTMPath(string source, string path)
        {
            var tokens = path.Split('/');
            var lastpart = tokens[3].Split(':');
            return "/IEDs/" + source + "/" + source + "&&&" + source + tokens[0] + "/" + tokens[1] + tokens[2] + lastpart[0] + "." + lastpart[1];
        }
    }
}
