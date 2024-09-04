using Microsoft.AspNetCore.Mvc;

namespace AppSettingsEncryption.AspNetCoreExample.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ExampleController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public string Get()
    {
        var val = _configuration.GetValue<string>("SettingToEncrypt")!;
        return val;
    }
}