## Dependecies
```powershell
# For both projects
dotnet add package Microsoft.Extensions.Hosting --version 7.0.1
dotnet add package Microsoft.Extensions.DependencyInjection --version 7.0.0
# Only for VkSignBot
dotnet add package VkNet --version 1.74.0
```

## Configuration
Inside the appsettings.json fill up these options.
```json
{
    "Vk": {
        "BotToken": "",
        "AppToken": "",
        "AppId": 0,
        "GroupId": 0
    }
}
```