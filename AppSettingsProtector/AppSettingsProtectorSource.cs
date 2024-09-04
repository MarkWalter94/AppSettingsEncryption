using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

namespace AppSettingsProtector;

public class AppSettingsProtectorSource : JsonConfigurationSource
{
    internal string ProtectionPrefix { get; }
    internal string ProtectedPrefix { get; }

    public AppSettingsProtectorSource(IFileProvider? fileProvider, string fileName, bool optional, string protectionPrefix = "|_@@_|", string protectedPrefix = "|@__@|")
    {
        this.FileProvider = fileProvider;
        this.Optional = optional;
        this.Path = fileName;
        ProtectionPrefix = protectionPrefix;
        ProtectedPrefix = protectedPrefix;
        ResolveFileProvider();
    }
    

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new AppSettingsProtectorProvider(this);
    }
}