using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Infrastructure.Services;

public class SharedResource
{

}
public sealed class LanguageService
{
    private readonly IStringLocalizer _localizer;

    public LanguageService(IStringLocalizerFactory factory)
    {
        var type = typeof(SharedResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = factory.Create(nameof(SharedResource), assemblyName.Name);
    }

    public string GetKey(string key)
    {
        var localizedString = _localizer[key];
        return localizedString.ResourceNotFound ? $"[{key}]" : localizedString.Value;
    }
}

