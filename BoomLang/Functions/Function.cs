using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang.Functions
{
    public class Function
    {
        public string name;
        public int minArgs = 0;
        public string dependency;
        public Function(string name, int minArgs)
        {
            this.name = name;
            this.minArgs = minArgs;
            this.dependency = "boomlang";
        }
        public Function(string name, int minArgs, string dependency)
        {
            this.name = name;
            this.minArgs = minArgs;
            this.dependency = dependency;
        }
        public virtual string GetAssembly(List<dynamic> args)
        {
            return "";
        }
    }
}
