using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MailService.Infrastructure.Tests.Security;

public class TestConfiguration : IConfiguration
{
 private readonly Dictionary<string, string> _values;

 public TestConfiguration(Dictionary<string, string> values)
 {
 _values = values;
 }

 public string this[string key]
 {
 get => _values[key];
 set => _values[key] = value;
 }

 public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();
 public IChangeToken GetReloadToken() => new CancellationChangeToken(new CancellationToken());
 public IConfigurationSection GetSection(string key) => throw new NotImplementedException();
}
