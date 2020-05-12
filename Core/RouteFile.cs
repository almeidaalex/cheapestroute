using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheapestTravel
{
    public static class RouteFile
    {
        public static IEnumerable<string> ReadFile(string path)
        {
            var lines = new List<string>();
            using var reader = new StreamReader(path);
            {
                while (!reader.EndOfStream)
                    lines.Add(reader.ReadLine());
            }

            return lines;
        }

        public static ImportResult WriteFile(string path, string route)
        {
            var regex = new Regex(FlightNetwork.LINE_PATTERN);
            var result = new ImportResult();
            if (regex.IsMatch(route))
            {
                using var writer = File.AppendText(path);
                writer.WriteLineAsync(route);
            }
            else
                result.AddError($"Input format mismatch {route}");

            return result;
        }

    }   

    public class ImportResult : Result {}
}
