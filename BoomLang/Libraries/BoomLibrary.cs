using BoomLang.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang.Libraries
{
    public class BoomLibrary
    {
        string name;
        public List<string> externs;
        public List<Function> allowedFunctions;
        public BoomLibrary(string name, List<string> externs, List<Function> allowedFunctions)
        {
            this.name = name;
            this.externs = externs;
            this.allowedFunctions = allowedFunctions;
        }

        public static Dictionary<string, BoomLibrary> libraries = new();
        public static void InitLibraries()
        {
            libraries.Clear();
            AddLibrary(new BasicIO());
        }
        public static void AddLibrary(BoomLibrary lib)
        {
            libraries.Add(lib.name, lib);
        }
        public static BoomLibrary GetBoomLibrary(string name)
        {
            try
            {
                return libraries[name];
            } catch (Exception)
            {
                AssemblyConverter.Error($"Could not find Library: {name}");
            }
            return null;
        }
    }
}
