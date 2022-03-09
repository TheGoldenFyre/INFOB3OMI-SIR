using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSIR_Console
{
    internal class Utils
    {
        internal static void getBool(string v, ref bool existing)
        {
            bool? temp = null;
            char[] options = new Regex(@"\[([A-Za-z0-9_]+)\]").Matches(v).Select(x => x.Value[1]).ToArray();
            while (temp == null)
            {
                Console.Write(v);
                string inp = Console.ReadLine();
                if (inp.ToUpper() == options[0].ToString()) temp = true;
                if (inp.ToUpper() == options[1].ToString()) temp = false;
            }
            existing = (bool)temp;
        }

        public static void getVar(string name, ref double x, double min)
        {
            double temp = min - 1;
            while (temp == min - 1)
            {
                Console.Write($"Enter {name} value ([D]efault {x}): ");
                string inp = Console.ReadLine();
                if (inp == "D" || inp == "d") temp = x;
                try { temp = double.Parse(inp, CultureInfo.InvariantCulture); temp = (temp > min) ? temp : -1; }
                catch { }
            }
            x = temp;
        }

        public static void getVar(string name, ref int x, int min)
        {
            int temp = min - 1;
            while (temp == min - 1)
            {
                Console.Write($"Enter {name} value ([D]efault {x}): ");
                string inp = Console.ReadLine();
                if (inp == "D" || inp == "d") temp = x;
                try { temp = int.Parse(inp, CultureInfo.InvariantCulture); temp = (temp > min) ? temp : -1; }
                catch { }
            }
            x = temp;
        }
    }
}
