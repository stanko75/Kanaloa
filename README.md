# Kanaloa
Live GPS tracking and travel blog assistent

To run the .NET application, you will need to create the appsettings.json file from \Kanaloa\\.net\webApi\Kanaloa\ and the RootUrl, which looks like this

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
To start Pics2gMaps build only Pics2gMaps, not Pics2gMaps.Test, and at least once open Settings.settings in designer

In \Kanaloa\html\templateForBlog index.html should be added, and it looks like:

```html
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:fb="http://ogp.me/ns/fb#">

<head>
    <meta content="text/html; charset=UTF-8" name="Content-Type" />
    <meta name="twitter:card" content="summary" />
    <meta name="twitter:site" content="@milosev.com" />
    <meta name="twitter:creator" content="@stankomilosev" />
    <meta property="og:title" content="/*ogTitle*/" />
    <meta property="og:url" content="/*ogUrl*/" />
    <meta property="og:image" content="/*ogImageFullPath*/" />
    <meta property="fb:app_id" content="myFbAppId" />
    <meta property="og:description" content="/*ogDescription*/" />
    <meta content="width=device-width, initial-scale=1" name="viewport" />
    <link type="text/css" rel="stylesheet" href="css/index.css" />
    <script defer type="text/javascript" src="lib/jquery-3.6.4.js"></script>
    <script defer type="text/javascript" src="script/namespaces.js"></script>
    <script defer type="text/javascript" src="script/map.js"></script>
    <script defer type="text/javascript" src="script/pics2maps.js"></script>
    <script defer type="text/javascript" src="script/thumbnails.js"></script>
    <script defer type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=myGoogleMapsKey&callback=window.milosev.initMap"></script>

</head>

<body>
    <div id="map-canvas"></div>
    <div id="thumbnails"></div>
</body>

</html>
```
