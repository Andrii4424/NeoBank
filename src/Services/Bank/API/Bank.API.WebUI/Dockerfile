FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src

COPY ["src/Services/Bank/API/Bank.API.WebUI/Bank.API.WebUI.csproj", "src/Services/Bank/API/Bank.API.WebUI/"]
COPY ["src/Services/Bank/API/Bank.API.Application/Bank.API.Application.csproj", "src/Services/Bank/API/Bank.API.Application/"]
COPY ["src/Services/Bank/API/Bank.API.Domain/Bank.API.Domain.csproj", "src/Services/Bank/API/Bank.API.Domain/"]
COPY ["src/Services/Bank/API/Bank.API.Infrastructure/Bank.API.Infrastructure.csproj", "src/Services/Bank/API/Bank.API.Infrastructure/"]

RUN dotnet restore "src/Services/Bank/API/Bank.API.WebUI/Bank.API.WebUI.csproj"

COPY . .

WORKDIR "/src/Services/Bank/API/Bank.API.WebUI"

RUN dotnet build "Bank.API.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "Bank.API.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bank.API.WebUI.dll"]
