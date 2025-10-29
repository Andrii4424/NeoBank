FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:7281
EXPOSE 7281

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Services/TransactionsService/Transactions.WebUI/Transactions.WebUI.csproj", "Services/TransactionsService/Transactions.WebUI/"]
COPY ["src/Services/TransactionsService/Transactions.Application/Transactions.Application.csproj", "Services/TransactionsService/Transactions.Application/"]
COPY ["src/Services/TransactionsService/Transactions.Domain/Transactions.Domain.csproj", "Services/TransactionsService/Transactions.Domain/"]
COPY ["src/Services/TransactionsService/Transactions.Infrastructure/Transactions.Infrastructure.csproj", "Services/TransactionsService/Transactions.Infrastructure/"]

RUN dotnet restore "Services/TransactionsService/Transactions.WebUI/Transactions.WebUI.csproj"

COPY src/ ./

WORKDIR "/src/Services/TransactionsService/Transactions.WebUI"
RUN dotnet build "Transactions.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Services/TransactionsService/Transactions.WebUI"
RUN dotnet publish "Transactions.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Transactions.WebUI.dll"]