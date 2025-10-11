# Kanaloa
Live GPS tracking and travel blog assistent

To run the .NET application, you will need to create the appsettings.json file from \Kanaloa\.net\webApi\Kanaloa\ and the RootUrl, which looks like this

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "KanaloaSettings": {
    "RootUrl": "https://www.kanaloa.com/" //here write localhost, or web site where kanalo will be published, this info is later used in config.json for live tracking
  }
}
```
