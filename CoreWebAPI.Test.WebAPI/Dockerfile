#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CoreWebAPI.Test.WebAPI/CoreWebAPI.Test.WebAPI.csproj", "CoreWebAPI.Test.WebAPI/"]
COPY ["CoreWebAPI.Extensions/CoreWebAPI.Extensions.csproj", "CoreWebAPI.Extensions/"]
COPY ["CoreWebAPI.Common/CoreWebAPI.Common.csproj", "CoreWebAPI.Common/"]
COPY ["CoreWebAPI.Test.Services/CoreWebAPI.Test.Services.csproj", "CoreWebAPI.Test.Services/"]
COPY ["CoreWebAPI.Test.IServices/CoreWebAPI.Test.IServices.csproj", "CoreWebAPI.Test.IServices/"]
COPY ["CoreWebAPI.Repository/CoreWebAPI.Repository.csproj", "CoreWebAPI.Repository/"]
COPY ["CoreWebAPI.Test.Models/CoreWebAPI.Test.Models.csproj", "CoreWebAPI.Test.Models/"]
RUN dotnet restore "CoreWebAPI.Test.WebAPI/CoreWebAPI.Test.WebAPI.csproj"
COPY . .
WORKDIR "/src/CoreWebAPI.Test.WebAPI"
RUN dotnet build "CoreWebAPI.Test.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreWebAPI.Test.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreWebAPI.Test.WebAPI.dll"]