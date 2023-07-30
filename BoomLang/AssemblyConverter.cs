using BoomLang.Functions;
using BoomLang.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoomLang
{
    public class AssemblyConverter
    {
        public static string BaseAssembly = "[EXTERN]\r\nglobal _main\r\n\r\nsection .data\r\n[DATA]\r\n\r\nsection .text\r\n_main:\r\n[MAIN]";
        public static List<BoomLibrary> importedLibraries = new();

        //list of strings defined in .data, where the key (first arg) is the value, and the second is the assembly name for the variable. done this way not to repeat variables
        public static Dictionary<string, string> definedDataStrings = new Dictionary<string, string>();

        public static void Clear()
        {
            BoomLibrary.InitLibraries();
            definedDataStrings = new();
            importedLibraries = new();
        }

        public static string ConvertAssembly(string boomLocation)
        {
            // init variables needed for conversion
            string assemblyOutput = Path.GetTempPath() + Guid.NewGuid().ToString() + ".asm";
            string generatedAssembly = BaseAssembly;
            List<string> externImports = new List<string>();
            StreamReader boomReader = new StreamReader(boomLocation);

            //read imports at beginning of file (imports BoomLibraries, not manually importing externs from assembly)
            string boomLine = boomReader.ReadLine();
            while (boomLine.StartsWith("import ") && boomLine != null)
            {
                string libName = boomLine.Substring("import ".Length).Trim();
                foreach (string asmExtern in BoomLibrary.GetBoomLibrary(libName).externs) {
                    externImports.Add(asmExtern);
                }
                importedLibraries.Add(BoomLibrary.GetBoomLibrary(libName));
                boomLine = boomReader.ReadLine();
            }

            //interpret actual function lines
            string asmCode = "";
            while (boomLine != null)
            {
                string functionName = boomLine.Substring(0, boomLine.IndexOf("("));
                Function function = GetFunctionByName(functionName);
                List<dynamic> args = GetArgumentsList(boomLine);

                string funcCode = function.GetAssembly(args);
                asmCode += funcCode + "\n";

                boomLine = boomReader.ReadLine();
            }

            //finally, add everything compiled to generated assembly to be passed off to NASM
            string externs = "";
            foreach (string asmExtern in externImports)
            {
                //add a variable to the assembly in data, this cannot be modified, I believe.
                externs += $"extern {asmExtern}\n";
            }
            generatedAssembly = generatedAssembly.Replace("[EXTERN]", externs);

            string dataStrings = "";
            foreach (KeyValuePair<string,string> variable in definedDataStrings)
            {
                dataStrings += $"{variable.Value}: db \"{variable.Key}\",10,0\n";
            }
            // still bad?
            generatedAssembly = generatedAssembly.Replace("[DATA]", dataStrings);

            //tab out asmCode and then add it to main
            asmCode = AddTabs(asmCode, 1);
            generatedAssembly = generatedAssembly.Replace("[MAIN]", asmCode);

            File.WriteAllText(assemblyOutput, generatedAssembly);
            return assemblyOutput;
        }

        public static string GetOrCreateDataVariable(string value)
        {
            if (definedDataStrings.ContainsKey(value))
            {
                return definedDataStrings[value];
            }
            string chars = "abcdefghijklmnopqrstuvwxyz";
            string randomName = "";
            Random random = new Random();
            for (int c = 0; c < 30; c++)
            {
                randomName += chars[random.Next(0, 26)];
            }
            definedDataStrings.Add(value, randomName);
            return randomName;
        }

        private static string AddTabs(string lines, int count)
        {
            string[] lineList = lines.Split("\n");
            string tabbedlines = "";
            foreach (string line in lineList)
            {
                string tabs = "";
                for (int i = 0; i < count; i++)
                {
                    tabs += "    ";
                }
                tabbedlines += $"{tabs}{line}\n";
            }
            return tabbedlines;
        }

        public static Function GetFunctionByName(string functionName)
        {
            foreach (BoomLibrary library in importedLibraries)
            {
                foreach (Function function in library.allowedFunctions)
                {
                    if (function.name.Equals(functionName))
                    {
                        return function;
                    }
                }
            }
            Error($"Function {functionName} did not exist in the current context.");
            return null;
        }

        private static List<dynamic> GetArgumentsList(string input)
        {
            List<dynamic> arguments = new List<dynamic>();

            int startIndex = input.IndexOf('(') + 1;
            int endIndex = input.LastIndexOf(')');
            string argumentsStr = input.Substring(startIndex, endIndex - startIndex);

            string[] args = argumentsStr.Split(',');
            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i].Trim();

                if (argument.StartsWith("\"") && argument.EndsWith("\""))
                {
                    arguments.Add(argument.Substring(1, argument.Length - 2));
                }
                else
                {
                    arguments.Add(argument);
                }
            }

            return arguments;
        }

        public static void Error(string errorMessage)
        {
            Console.WriteLine($"Error: {errorMessage}");
            Console.WriteLine("Closing in 10 seconds...");
            Thread.Sleep(10000);
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
