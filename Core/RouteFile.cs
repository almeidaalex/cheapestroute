using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace CheapestTravel
{
  public static class RouteFile
  {
    public static IReadOnlyCollection<string> ReadFile(string path)
    {
      var lines = new List<string>();
      using var reader = new StreamReader(path);
      while (!reader.EndOfStream)
        lines.Add(reader.ReadLine());

      return lines.AsReadOnly();
    }

    public static ImportResult WriteFile(string path, params string[] routes)
    {
      var result = new ImportResult();
      File.WriteAllLines(path, routes);
      return result;
    }

  }

  public class ImportResult : Result { }
}
