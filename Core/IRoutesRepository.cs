using System.Collections.Generic;

namespace Core;

public interface IRoutesRepository
{
  bool HasHub(string hubName);

  void AddRoute(string source, string destination, uint cost);

  IReadOnlyDictionary<string, IReadOnlyDictionary<string, uint>> GetAll();

  IDictionary<string, uint> Get(string key);

  void Save();
}
