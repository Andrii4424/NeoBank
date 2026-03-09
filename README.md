# Neobank
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

## License
This project is licensed under the **MIT License**.  
See the [LICENSE](LICENSE) file for details.

## Contact Information

For any questions, suggestions, or feedback, please use GitHub Issues or contact us through the project's GitHub page.  
You can find the repository at [GitHub Repository](https://github.com/Andrii4424/NeoBank).

We look forward to your contributions and thank you for being a part of the NeoBank community!

## Project Presentation

### Pages for unauthorized user
- Home (two languages)
  
![Home](docs/images/unlogged/home.jpg)
![Home-UA](docs/images/unlogged/home-ua.jpg)

- Bank Info

![Bank-Info](docs/images/unlogged/bank-info.jpg)

- Cards

![Card-Tariffs](docs/images/unlogged/card-tariffs-page.jpg)
![Card-Tariffs-Info](docs/images/unlogged/card-tariffs-page-info.jpg)

- Deposits

![Deposits](docs/images/unlogged/deposits-page.jpg)

- Credits

![Deposits](docs/images/unlogged/credits-page.jpg)

- Vacancies

![Vacancies](docs/images/unlogged/vacancies-page.jpg)

- Currency Rates

![Currency-Rates](docs/images/unlogged/currency-rates-page.jpg)

### Authorization

- Log In
  
![Log-In](docs/images/auth/login-page.jpg)

- Registering
  
![Registering](docs/images/auth/register-page.jpg)

- Recovery Password

![Recovery-Password-Enter-Email](docs/images/auth/reset-password-enter-email.jpg)
![Recovery-Password-Enter-Code](docs/images/auth/reset-password-enter-code.jpg)
![Recovery-Password-Enter-Password](docs/images/auth/reset-password-enter-new-password.jpg)

### Profile

- Not Verified Profile

![Not-Verified-Profile](docs/images/profile/not-verified-own-profile.jpg)

- Updating Profile

![Updating-Profile](docs/images/profile/updating-profile.jpg)
![Updating-Profile](docs/images/profile/updated-profile.jpg)

- Applying for job (user gets admin role, when applies for job)

![Vacancies](docs/images/profile/vacancies-page-verified.jpg)
![Apply-For-Job](docs/images/profile/applying-for-job.jpg)

### My Cards

- My Cards Page

![My-Cards-Empty](docs/images/admin/my-cards-page.jpg)
![My-Cards-With-Cards](docs/images/admin/my-cards-page-with-cards.jpg)
![My-Cards-With-Blocked-Card](docs/images/admin/my-cards-with-blocked-card.jpg)

- My Cards Info Page

![My-Cards-Info](docs/images/admin/my-card-info.jpg)

### Card Actions

- Add Funds
![Add-Funds](docs/images/admin/add-funds.jpg)
![Add-Funds-Result](docs/images/admin/add-funds-result.jpg)

- Change Pin
![Change-Pin](docs/images/admin/change-pin.jpg)
![Change-Pin-Result](docs/images/admin/change-pin-result.jpg)

- Reissue Card

![Reissue-Card](docs/images/admin/reissue-card.jpg)
![Reissue-Card-Result](docs/images/admin/reissue-card-result.jpg)

- Block Card

![Block-Card](docs/images/admin/block-card.jpg)
![Block-Card-Result](docs/images/admin/block-card-result.jpg)

- Close Card

![Close-Card](docs/images/admin/close-card.jpg)

### Transactions

- P2P Transaction

![P2P-Transaction](docs/images/admin/make-p2p-transaction.jpg)
![P2P-Transaction-Result](docs/images/admin/make-p2p-transaction-result.jpg)
  
- Exchange Currency

![Exchange-Transaction](docs/images/admin/exchange-currency.jpg)
![Exchange-Transaction-Result](docs/images/admin/exchange-currency-result.jpg)
  
- Transaction History

![Transaction History](docs/images/admin/transactions-history.jpg)  

### Admins pages

- Bank Info

![Bank-Info-Admin](docs/images/admin/bank-info-admin-page.jpg)

- Update Bank Info

![Update-Bank-Info-Admin](docs/images/admin/update-bank-info.jpg)

- Card Tariffs

![Card-Tariffs-Admin](docs/images/admin/card-tariffs-info-admin-page.jpg)

- Card Tariffs Info

![Card-Tariffs-Admin](docs/images/admin/card-tariffs-info-admin-page.jpg)

- Update Card Tariffs
![Update-Card-Tariffs-Admin](docs/images/admin/update-card-tariffs.jpg)

- Delete Card Tariffs
![Delete-Card-Tariffs-Admin](docs/images/admin/delete-card-tariffs.jpg)

- Vacancies Page

![Vacancies-Page](docs/images/admin/admins-vacancies-page.jpg)

- Add Vacancy

![Add-Vacancy](docs/images/admin/add-vacancy.jpg)

- Update Vacancy

![Update-Vacancy](docs/images/admin/update-vacancy.jpg)

- Delete Vacancy

![Delete-Vacancy](docs/images/admin/delete-vacancy.jpg)




