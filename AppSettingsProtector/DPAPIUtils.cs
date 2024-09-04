using System.Security.Cryptography;
using System.Text;

#pragma warning disable CA1416
public class DPAPIUtils
{
    private static string Protect(string stringToEncrypt, string optionalEntropy)
    {
        return Convert.ToBase64String(
            ProtectedData.Protect(
                Encoding.UTF8.GetBytes(stringToEncrypt)
                , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                , DataProtectionScope.LocalMachine));
    }

    private static string Unprotect(string encryptedString, string optionalEntropy)
    {
        return Encoding.UTF8.GetString(
            ProtectedData.Unprotect(
                Convert.FromBase64String(encryptedString)
                , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
                , DataProtectionScope.LocalMachine));
    }

    public static string Encrypt(string stringToEncrypt, string optionalEntropy)
    {
        string encEnt = string.Empty;
        string encVal = string.Empty;
        do
        {
            encEnt = Protect(optionalEntropy, "sda|#!%(_@#!$@#$%e4b7iofvto27i834yrtp9q4whepi!@#%$@%");
            encVal = Protect(stringToEncrypt, optionalEntropy);
        } while (encEnt.Contains("qq") || encVal.Contains("qq"));

        var encrypted = $"{encVal}qq{encEnt}";

        do
        {
            encEnt = Protect(optionalEntropy, "qjwyhkebfkuy3q4b$FGW$(HG)*#$P%");
            encVal = Protect(encrypted, optionalEntropy);
        } while (encEnt.Contains("qqzz") || encVal.Contains("qqzz"));

        return $"{encVal}qqzz{encEnt}";
    }

    public static string Decrypt(string stringToDecrypt)
    {
        var val = stringToDecrypt.Split("qqzz")[0];
        var entr = stringToDecrypt.Split("qqzz")[1];
    var visEntr = Unprotect(entr, "qjwyhkebfkuy3q4b$FGW$(HG)*#$P%");
        var decr1 = Unprotect(val, visEntr);

        val = decr1.Split("qq")[0];
        entr = decr1.Split("qq")[1];
        visEntr = Unprotect(entr, "sda|#!%(_@#!$@#$%e4b7iofvto27i834yrtp9q4whepi!@#%$@%");
        return Unprotect(val, visEntr);
    }
}
#pragma warning restore CA1416