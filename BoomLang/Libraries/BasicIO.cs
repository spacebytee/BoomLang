using BoomLang.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang.Libraries
{
    public class BasicIO : BoomLibrary
    {
        public BasicIO() : base("BasicIO", externs, functions)
        {

        }

        public static List<string> externs = new List<string>() { "_printf" };
        public static List<Function> functions = new List<Function>() { new Print() };

    }
}
