using System.Collections.Generic;

namespace Core;

public interface IRoutesRepository
{
  void AddRoute(string source, string destination, uint cost);

  IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> GetAll();

  void Save();
}
