using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang.Functions
{
    public class Print : Function
    {
        public Print() : base("print", 1, "BasicIO")
        {

        }

        public override string GetAssembly(List<dynamic> args)
        {
            string variable = AssemblyConverter.GetOrCreateDataVariable(args[0]);
            return $"push {variable}\r\ncall _printf\r\nadd esp,4";
        }
    }
}
