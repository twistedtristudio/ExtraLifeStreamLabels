# ExtraLifeStreamLabels
.NET Core Extra Life Stream Labels Service

Extra Life Stream Labels is a service that utilizes the Donor Drive public API to fetch Extra Life participant, team, and donation information.

To install the service, simply run the MSI.

After the install is complete, add your Participant Id, Team Id, and desired location for stream labels to be saved to the appsettings.json file located at C:\Program Files (x86)\WireJunky Solutions.

```javascript
  "ExtraLifeData": {
    "ParticipantId": "400475",
    "TeamId": "",
    "StreamLabelOutputPath": "D:\\ExtraLifeStreamLabelData_Core\\"
  }
```

Like the work I have done?  Consider making a small donation to my Extra Life campaign at https://crits4kids.info/ExtraLifePage


![Release (Master)](https://github.com/WireJunky-Solutions/ExtraLifeStreamLabels/workflows/Release%20(Master)/badge.svg?branch=master)

![Development](https://github.com/WireJunky-Solutions/ExtraLifeStreamLabels/workflows/Development/badge.svg?branch=develop)
