# VH Test Api

The Test Api supports the acceptance tests in the vh-video-web, vh-service-web and vh-admin-web projects by centralising the management of test users and data, as well as providing a mechanism for the front end tests to retreive data from the backend vh-booking-api, vh-video-api and vh-user-api.

## Current Build Status
[![Build Status](https://dev.azure.com/hmctsreform/VirtualHearings/_apis/build/status/hmcts.vh-test-api?branchName=master)](https://dev.azure.com/hmctsreform/VirtualHearings/_build/latest?definitionId=120&branchName=master)

## Quality Gate Status
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vh-test-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=vh-test-api)

## Generating the clients
If the interface for the Bookings API Client, Video API Client or User API Client are missing after cloning the repo, these can be rebuilt using the following command:

In the `TestApi.Services.Generator` folder:
```
nswag run
```

## Branch name git hook will run on pre commit and control the standard for new branch name.

The branch name should start with: feature/VIH-XXXX-branchName (X - is digit).
If git version is less than 2.9 the pre-commit file from the .githooks folder need copy to local .git/hooks folder.
To change git hooks directory to directory under source control run (works only for git version 2.9 or greater) :
\$ git config core.hooksPath .githooks

## Commit message

The commit message will be validated by prepare-commit-msg hook.
The commit message format should start with : 'feature/VIH-XXXX : ' folowing by 8 or more characters description of commit, otherwise the warning message will be presented.

## Run Stryker

To run stryker mutation test, go to UnitTest folder under command prompt and run the following command

```bash
dotnet stryker
```

From the results look for line(s) of code highlighted with Survived\No Coverage and fix them.

If in case you have not installed stryker previously, please use one of the following commands

## Global
```bash
dotnet tool install -g dotnet-stryker
```
## Local
```bash
dotnet tool install dotnet-stryker
```

To update latest version of stryker please use the following command

```bash
dotnet tool update --global dotnet-stryker
```

## Run Zap scan locally

To run Zap scan locally update the following settings and run acceptance\integration tests

Update following configuration under appsettings.json under TestApi.IntegrationTests

- "Services:TestApiUrl": "https://TestApi_AC/"
- "ZapConfiguration:ZapScan": true
- "ConnectionStrings:TestApi": "Server=localhost,1433;Database=TestApi;User=sa;Password=VeryStrongPassword!;" (TestApi\appsettings.development.json)

Note: Ensure you have Docker desktop engine installed and setup

