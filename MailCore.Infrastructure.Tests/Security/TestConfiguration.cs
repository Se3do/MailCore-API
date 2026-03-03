using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MailCore.Infrastructure.Tests.Security;

public class TestConfiguration : IConfiguration
{
    private readonly Dictionary<string, string> _values;

    public TestConfiguration(Dictionary<string, string> values)
    {
        _values = values;
    }

    public string? this[string key]
    {
        get => _values.TryGetValue(key, out var v) ? v : null;
        set => _values[key] = value!;
    }

    public IEnumerable<IConfigurationSection> GetChildren() => [];

    public IChangeToken GetReloadToken() => new CancellationChangeToken(new CancellationToken());

    public IConfigurationSection GetSection(string key) =>
        new TestConfigurationSection(_values, key);
}

public class TestConfigurationSection : IConfigurationSection
{
    private readonly Dictionary<string, string> _values;
    private readonly string _prefix;

    public TestConfigurationSection(Dictionary<string, string> values, string prefix)
    {
        _values = values;
        _prefix = prefix;
    }

    public string? this[string key]
    {
        get => _values.TryGetValue($"{_prefix}:{key}", out var v) ? v : null;
        set => _values[$"{_prefix}:{key}"] = value!;
    }

    public string Key => _prefix.Split(':').Last();
    public string Path => _prefix;
    public string? Value
    {
        get => _values.TryGetValue(_prefix, out var v) ? v : null;
        set => _values[_prefix] = value!;
    }

    public IEnumerable<IConfigurationSection> GetChildren() => [];
    public IChangeToken GetReloadToken() => new CancellationChangeToken(new CancellationToken());
    public IConfigurationSection GetSection(string key) =>
        new TestConfigurationSection(_values, $"{_prefix}:{key}");
}
