`AppSettings.Encryption` is a simple yet powerful library that protects the data inside your settings using the `Data Protection API` provided by .NET Core.
The library uses DPAPI, that ensures that the data is decryptable only on the machine that encrypted it.

# Usage

1. Install the package via NuGet:
```bash
dotnet add package AppSettings.Encryption
```

2. Add the following code to your `Program.cs` file:

```csharp 
builder.Configuration.Sources.Clear();
builder.Configuration.AddProtectedJsonFile();
```

3. To encrypt a setting value use the encryption prefix in you settings file:
    Say you have the following settings file:
```json
{
  "SettingToEncrypt": "This is a value that will be encrypted"
}
```
To encrypt the value of `SettingToEncrypt` you can use the default encryption prefix `|_@@_|`:
```json
{
  "|_@@_|SettingToEncrypt": "This is a value that will be encrypted"
}
```

4. After the first run of your application, the library will automatically encrypt the value of `SettingToEncrypt` and update the settings file:
```json
{
  "|@__@|SettingToEncrypt": "AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAA3J"
}
```

5. Now you can access the decrypted value using the usual .NET Core configuration API:
```csharp
var settingValue = _configuration.GetValue<string>("SettingToEncrypt");
```

# Examples
You can find a simple example opening the `AppSettingsEncryption.AspNetCoreExample` project.
More examples will be added in the future.

# Other configurations
### Custom settings file path
You can specify the path to the settings file by passing it as a parameter to the `AddProtectedJsonFile` method:
```csharp
builder.Configuration.AddProtectedJsonFile("customnameappsettings.json");
```

You can also specify the entire file provider:
```csharp
builder.Configuration.AddProtectedJsonFile(new PhysicalFileProvider(Directory.GetCurrentDirectory()), "customnameappsettings.json", false);
```

### Custom encryption prefix
The encryption prefix (`|_@@_|` by default) and encrypted value prefix (`|@__@|` by default) can be customized choosing one of the overloads of the `AddProtectedJsonFile` method:
```csharp
builder.Configuration.AddProtectedJsonFile("customEncryptionPrefix", "customEncryptedValuePrefix");
```

# Remarks
- Up to now the library is designed to work with JSON settings files only.
- The library requires the access to the settings file to be able to encrypt the values in the first run. If the file is read-only or if the program has no write access to it, the library will throw an exception. This can be solved by executing the app with admin rights on the first run.