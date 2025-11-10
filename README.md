### Neobank
## Deployment
- https://neobank.switzerlandnorth.cloudapp.azure.com/
## Description
NeoBank is a full-stack online banking system that simulates real-world financial operations such as card management, currency exchange, and P2P transactions within a scalable microservice architecture.

Users can create personal accounts, manage their finances, and interact with the platform both as clients and contributors.
They can apply for open positions within the bank to become part of the internal team or advance to the administrator level, gaining access to extended management functionality including system monitoring, user control, and tariff configuration.

## Features
- **User Accounts:** Register, log in, and manage personal profiles and financial data.
- **Card Management:** Create, block, and manage virtual cards with real-time status updates.
- **Currency Exchange:** View current exchange rates and convert funds between currencies.
- **P2P Transfers:** Instantly send and receive money between users inside the system.
- **Transaction History:** Track all past transfers and card operations in a detailed list.
- **Job Applications:** Browse and apply for open internal bank positions directly from your account.
- **Admin Dashboard:** Manage bank, add vacancies, and configure card tariffs and permissions.
- **Localization:** Switch interface languages (English / Ukrainian).
- **Security:** Role-based access control and JWT authentication.
- **Dockerized Deployment:** Entire project runs with one command using Docker Compose.

## Technologies
### Backend
- **.NET 8**
- **ASP.NET Core**
- **Web API**
- **Entity Framework Core**
- **Microsoft SQL Server**
- **Serilog**
- **XUnit**
- **Email Sender (SMTP)**
- **AutoMapper**
- **Swagger**
- **RabbitMq**
### Frontend
- **Angular 20**
- **RxJS & Signals**
- **SCSS**
- **ngx-cookie-service**
- **ngx-translate**
### Others
- **Docker**
- **Nginx**
- **GitHub Actions**
- **Certbot**

## Installation
To set up the project, follow these steps:

### 1. **Prerequisites:**
   - SQL Server  
   - .NET 8
   - RabbitMq

### 2. **Build the Project:**
- **Bank API**
  
   ```bash
   dotnet build src\Services\BankService\Bank.API.WebUI
-  **Transactions API**
  
   ```bash
   dotnet build src\Services\TransactionsService\Transactions.WebUI

### 3. Configure appsettings.json
- **appsettings.json (Bank API)**
   ```bash    
  {
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "DefaultConnection": "yourdatabaseconnection"
    },
    "AccessToken": {
      "Issuer": "https://localhost:7280",
      "Audience": "https://localhost:4200",
      "Expiration_minutes": 60,
      "Key": "Public development key for api authentication (must be longer than 256 bits)"
    },
    "RefreshToken": {
      "Expiration_days": 7
    },
    "AdminUser": {
      "Email": "admin@neobank.com",
      "Password": "Qwerty123!"
    },
    "TransactionRabbitMq": {
      "Host": "yourrabbithost",
      "Username": "yourrabbitusername",
      "Password": "yourrabbitpassword",
      "Port": "yourrabbitport",
      "VirtualHost": "/"
    },
  
    "Smtp": {
      "User": "youremail@example.com",
      "Pass": "youremailpassword",
      "Host": "smtp.gmail.com",
      "Port": "587",
      "From": "NeoBank"
    }
  }
- **appsettings.json (Transactions API)**
  ```bash
  {
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "DefaultConnection": "yourdatabaseconnection"
    },
    "RabbitMq": {
      "Host": "yourrabbithost",
      "Username": "yourrabbitusername",
      "Password": "yourrabbitpassword",
      "Port": "yourrabbitport",
      "VirtualHost": "/"
    },
    "BankApiUrl": "https://localhost:7280/api/"
  }
##Usage
To use the application, follow these steps:
  - **Bank API**
    
     ```bash
     dotnet run src\Services\BankService\Bank.API.WebUI
  -  **Transactions API**
    
     ```bash
     dotnet run src\Services\TransactionsService\Transactions.WebUI
  -  **Frontend**
    
     ```bash
     npm run build --prefix src\Client
  1. Start the Application:
  2. Once the application is running, you can access the web interface and interact with the available features. The API endpoints will also be available for external applications.

##Testing
- To run the tests for this project, use the following command:
  ```bash
      dotnet test
Testing Frameworks: **XUnit**

##License

##Contacts
