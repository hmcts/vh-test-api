# Create Base Image.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Publish .NET App.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS publish
WORKDIR /src
COPY TestApi .
RUN dotnet restore --force-evaluate --configfile nuget.config "TestApi/TestApi.csproj" 
WORKDIR "/src/TestApi"
RUN dotnet publish "TestApi.csproj" -c Release -o /app/publish

# Create Final App Image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApi.dll"]