using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace AppSettingsProtector;

public static class ConfigurationBuilderExtensions
{
    /// <summary>
    ///  Add the protection layer to the configuration file.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="fileProvider"></param>
    /// <param name="fileName"></param>
    /// <param name="optional"></param>
    /// <param name="protectionPrefix">Prefix that indicates that a setting must be encrypted.</param>
    /// <param name="protectedPrefix">Prefix that indicates that a setting is encrypted.</param>
    /// <returns></returns>
    public static IConfigurationBuilder AddProtectedJsonFile(this IConfigurationBuilder configurationBuilder, IFileProvider? fileProvider, string fileName, bool optional, string protectionPrefix, string protectedPrefix)
    {
        var source = new AppSettingsProtectorSource(fileProvider, fileName, optional, protectionPrefix, protectedPrefix);
        return configurationBuilder.Add(source);
    }

    /// <summary>
    /// Add the protection layer to the configuration file using the default prefixes.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="fileProvider"></param>
    /// <param name="fileName"></param>
    /// <param name="optional"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddProtectedJsonFile(this IConfigurationBuilder configurationBuilder, IFileProvider? fileProvider, string fileName, bool optional)
    {
        var source = new AppSettingsProtectorSource(fileProvider, fileName, optional);
        return configurationBuilder.Add(source);
    }
    
    /// <summary>
    /// Add the protection layer to the configuration file using the default prefixes.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddProtectedJsonFile(this IConfigurationBuilder configurationBuilder, string fileName)
    {
        var source = new AppSettingsProtectorSource(null, fileName, false);
        return configurationBuilder.Add(source);
    }
    
    /// <summary>
    /// Add the protection layer to the configuration file using the default prefixes.
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <returns></returns>
    public static IConfigurationBuilder AddProtectedJsonFile(this IConfigurationBuilder configurationBuilder)
    {
        var source = new AppSettingsProtectorSource(null, "appsettings.json", false);
        return configurationBuilder.Add(source);
    }
}