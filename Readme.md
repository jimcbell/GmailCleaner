## Gmail Cleaner

### Technology Used
- Sql Server 
    - Storing users that log in through gmail, their access token, refresh token, etc.
    - Storing email messages that are recieved through the api and marked for deletion
- Entity Framework Core 
    - Object relational mapper for Gmail Cleaner Database
- OAuth
    - Gmail Cleaner is an app registered with Google's OAuth services, uses Authorization Code flow to get access tokens for users.
- Asp.Net Core Identity
    - Allowing Users to login in through gmail, and getting tokens/ refresh tokens to make api request
- Asp.Net Core Razor Pages
    - UI and making client side requests to server
- Terraform
    - Infrastructure as code used to create Azure resources


### Azure Infrastructure Used
- Azure Key Vault
    - Used to store secret configuration values
- Azure Sql Database
    - Relational database management system
- Azure App Service
    - Used to host the web application


### To Run 

install the dotnet ef tools on your computer 

run 
```dotnet ef migrations add {give a name}```
```dotnet ef databaase update```

If you are setting this up yourself you should just need to add your client id and secret for your app registered with google cloud platform
Add the same scopes that I am using
And either match the redirect uri that I have or add your own and make sure it is reflected in the Program.cs / appsettings

