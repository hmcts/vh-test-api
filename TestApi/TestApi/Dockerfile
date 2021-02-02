#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TestApi/TestApi.csproj", "TestApi/"]
COPY ["TestApi.Contract/TestApi.Contract.csproj", "TestApi.Contract/"]
COPY ["TestApi.Domain/TestApi.Domain.csproj", "TestApi.Domain/"]
COPY ["TestApi.Common/TestApi.Common.csproj", "TestApi.Common/"]
COPY ["TestApi.Services/TestApi.Services.csproj", "TestApi.Services/"]
COPY ["TestApi.Services.Generator/TestApi.Services.Generator.csproj", "TestApi.Services.Generator/"]
COPY ["TestApi.DAL/TestApi.DAL.csproj", "TestApi.DAL/"]
COPY ["nuget.config", "nuget.config"]
RUN dotnet restore --configfile nuget.config "TestApi/TestApi.csproj"
COPY . .
WORKDIR "/src/TestApi"
RUN dotnet build "TestApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApi.dll"]