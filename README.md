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

To run Android app, you will need to create \Kanaloa\android\kanaloa\app\src\main\res\raw\config.properties which look like this:

```
web.host=https://www.kanaloa.com //this info will be used for app to know where (to which API) to send gps info, pics,...
```

as well as \Kanaloa\android\kanaloa\secrets.properties which looks like:

```
sdk.dir=C\:\\Users\\username\\AppData\\Local\\Android\\Sdk
MAPS_API_KEY="myGoogleMapsKey"
```
