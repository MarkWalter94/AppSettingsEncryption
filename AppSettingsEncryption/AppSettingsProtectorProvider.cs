using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppSettingsEncryption;

public class AppSettingsProtectorProvider : JsonConfigurationProvider
{
    private readonly AppSettingsProtectorSource protectedSource;

    public AppSettingsProtectorProvider(JsonConfigurationSource source) : base(source)
    {
        this.protectedSource = (AppSettingsProtectorSource)source;
    }

    public override void Load(Stream stream)
    {
        base.Load(stream);
        DoProtectorStuff();
    }

    /// <summary>
    /// Executes the operations of obfuscation and deobfuscation.
    /// </summary>
    private void DoProtectorStuff()
    {
        var encryptedValues = new Dictionary<string, string>();
        var decryptedValues = new Dictionary<string, string>();
        var valToRemove = new List<string>();

        foreach (var setting in this.Data)
        {
            var splitted = setting.Key.Split(':');
            var settingEndingName = splitted.Last();
            if (settingEndingName.StartsWith(this.protectedSource.ProtectionPrefix) && !string.IsNullOrEmpty(setting.Value))
            {
                var settingEncryptedName = string.Join(':', splitted.Take(splitted.Length - 1)) + ":" + this.protectedSource.ProtectedPrefix + settingEndingName[this.protectedSource.ProtectionPrefix.Length..];
                var settingRealName = string.Join(':', splitted.Take(splitted.Length - 1)) + ":" + settingEndingName[this.protectedSource.ProtectedPrefix.Length..];

                if (settingEncryptedName[0] == ':')
                    settingEncryptedName = settingEncryptedName[1..];
                if (settingRealName[0] == ':')
                    settingRealName = settingRealName[1..];

                //Setting to encrypt!
                encryptedValues.Add(settingEncryptedName, DoCrypto(setting.Value));
                //Add the decrypted value to the dictionary.
                decryptedValues.Add(settingRealName, setting.Value);

                valToRemove.Add(setting.Key);
            }
            else if (settingEndingName.StartsWith(this.protectedSource.ProtectedPrefix) && !string.IsNullOrEmpty(setting.Value))
            {
                var settingRealName = string.Join(':', splitted.Take(splitted.Length - 1)) + ":" + settingEndingName[this.protectedSource.ProtectedPrefix.Length..];
                if (settingRealName[0] == ':')
                    settingRealName = settingRealName[1..];

                //Decrypt!
                var decryptedValue = DPAPIUtils.Decrypt(setting.Value);
                decryptedValues.Add(settingRealName, decryptedValue);
            }
        }

        if (encryptedValues.Count != 0)
        {
            SaveProperties(encryptedValues, valToRemove);
        }


        foreach (var val in decryptedValues)
            this.Set(val.Key, val.Value);
    }

    /// <summary>
    /// Saves the properties of the settings file.
    /// </summary>
    /// <param name="propsToSave"></param>
    /// <param name="propsToRem"></param>
    private void SaveProperties(IDictionary<string, string> propsToSave, IEnumerable<string> propsToRem)
    {
        var fileFullPath = base.Source.FileProvider!.GetFileInfo(base.Source.Path!).PhysicalPath;
        var json = File.ReadAllText(fileFullPath!);
        dynamic jsonObj = JObject.Parse(json);

        foreach (var val in propsToSave)
        {
            var splittedVal = val.Key.Split(':');
            if (splittedVal.Length == 1 || splittedVal[0].Length == 0)
                jsonObj[val.Key] = val.Value;
            else
            {
                var reobj = jsonObj;
                foreach (var spl in splittedVal.Take(splittedVal.Length - 1))
                {
                    //Go recursively to the inner member.
                    reobj = reobj[spl];
                }

                reobj[splittedVal.Last()] = val.Value;
            }
        }

        foreach (var val in propsToRem)
        {
            var splittedVal = val.Split(':');
            if (splittedVal.Length == 1)
                ((JObject)jsonObj).Remove(val);
            else
            {
                var reobj = jsonObj;
                foreach (var spl in splittedVal.Take(splittedVal.Length - 1))
                {
                    //Go recursively to the inner member.
                    reobj = reobj[spl];
                }

                ((JObject)reobj).Remove(splittedVal.Last());
            }
        }

        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
        File.WriteAllText(fileFullPath!, output);
    }

    #region Crypto
    private static readonly Random _random = new();

    private static string DoCrypto(string input)
    {
        return DPAPIUtils.Encrypt(input, RandomString(_random.Next(15, 40)));
    }

    private static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_-+=";
        return new string(Enumerable.Range(1, length).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
    }
    #endregion
}