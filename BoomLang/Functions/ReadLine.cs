using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang.Functions
{
    public class ReadLine : Function
    {
        public ReadLine() : base("readline", 1, "BasicIO")
        {

        }

        public override string GetAssembly(List<dynamic> args)
        {
            string variable = AssemblyConverter.GetOrCreateDataVariable(args[0]);
            return $"push {variable}\r\ncall _printf\r\nadd esp,4";
        }
    }
}
