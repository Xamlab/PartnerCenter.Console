# Overview

The ParnterCenter.Console repository contains a sample to utilize [Microsof Partner Center APIs](https://docs.microsoft.com/en-us/windows/uwp/monetize/create-and-manage-submissions-using-windows-store-services) to update packages.

## Running the sample

1. Follow the prequisite [steps](https://docs.microsoft.com/en-us/windows/uwp/monetize/create-and-manage-submissions-using-windows-store-services)
2. Ovveride the values in the `appsettings.json` file 
	- `Tenant` - the AAD tenant ID to which your app is associated with
	- `ApplicationId`- the ID of the application in the Partner Center
	- `ClientId` - the client ID of the application that you associated with your partner center account in the first step
	- `ClientSecret` - the client secret of the same app. You can also set the value of this in the project user secrets
3. Make sure you have UWP package ready
4. Use the following command to run the sample
```
 dotnet run --project PartnerCenter.Console\PartnerCenter.Console.csproj --flight 'Beta' --bundle 'PATH_TO\*.msixupload'
```