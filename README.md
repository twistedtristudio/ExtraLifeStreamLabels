# ExtraLifeStreamLabels
.NET Core Extra Life Stream Labels Service

Extra Life Stream Labels is a service that utilizes the Donor Drive public API to fetch Extra Life participant, team, and donation information.

Add your Participant Id, Team Id, and desired location for stream labels to be saved to the appsettings.json file.

```javascript
  "ExtraLifeData": {
    "ParticipantId": "400475",
    "TeamId": "",
    "StreamLabelOutputPath": "D:\\ExtraLifeStreamLabelData_Core\\"
  }
```

To install the service, open a command prompt as administrator and enter the following command:

sc create ExtraLifeStreamLabels_Core BinPath=**C:\full\path\to\publish\dir**\WJS.ExtraLifeStreamLabelsService.exe

The bolded portion of the path will be the path you have unzipped the release package

To start the service: sc start ExtraLifeStreamLabels_Core
To stop the service: sc stop ExtraLifeStreamLabels_Core
to delete the service: sc delete ExtraLifeStreamLabels_Core


![Release (Master)](https://github.com/WireJunky-Solutions/ExtraLifeStreamLabels/workflows/Release%20(Master)/badge.svg?branch=master)

![Development](https://github.com/WireJunky-Solutions/ExtraLifeStreamLabels/workflows/Development/badge.svg?branch=develop)
