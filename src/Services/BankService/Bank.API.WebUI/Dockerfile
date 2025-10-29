FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:7280
EXPOSE 7280

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Services/BankService/Bank.API.WebUI/Bank.API.WebUI.csproj", "Services/BankService/Bank.API.WebUI/"]
COPY ["src/Services/BankService/Bank.API.Application/Bank.API.Application.csproj", "Services/BankService/Bank.API.Application/"]
COPY ["src/Services/BankService/Bank.API.Domain/Bank.API.Domain.csproj", "Services/BankService/Bank.API.Domain/"]
COPY ["src/Services/BankService/Bank.API.Infrastructure/Bank.API.Infrastructure.csproj", "Services/BankService/Bank.API.Infrastructure/"]

RUN dotnet restore "Services/BankService/Bank.API.WebUI/Bank.API.WebUI.csproj"

COPY src/ ./

WORKDIR "/src/Services/BankService/Bank.API.WebUI"
RUN dotnet build "Bank.API.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Services/BankService/Bank.API.WebUI"
RUN dotnet publish "Bank.API.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bank.API.WebUI.dll"]
