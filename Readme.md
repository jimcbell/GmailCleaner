### To Do 

Refacor the email repository so that we do not have to load the access token

Fix up the UI and make it look better 

Use a .net package to parse out the base 64 encoded email payloads 

Move the sql database used for local development into the cloud with azure (use terraform and keep the tf files local)
- Setup the tf state and include the keyvault so that all the secrets can be pulled from there

Most important is setting up tf state and key vault

### To Run 

install the dotnet ef tools on your computer 

run 
```dotnet ef migrations add {give a name}```
```dotnet ef databaase update```

If you are setting this up yourself you should just need to add your client id and secret for your app registered with google cloud platform
Add the same scopes that I am using
And either match the redirect uri that I have or add your own and make sure it is reflected in the Program.cs / appsettings

