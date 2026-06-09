# Seawave Web API

This is a web API that acts as a backend for a music streaming platform. It registrates users, keeps track of music files, uploads and playlists, querries searches and streams a music file to the user.

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- Dapper
- MariaDB
- BCrypt
- MailKit

##How to Run Locally (For Developers)

### Prerequisites
- .NET 10 SDK or Later
- MariaDB Server

### Installation & Run
1. Clone the repository:
```bash
git clone [https://github.com/yourusername/your-repo.git](https://github.com/yourusername/your-repo.git)
```
2. Open appsettings.Development.json and place the values for your case. Do the same with appsettings.json.

3. Navigate to the project directory and restore dependencies:
```bash
dotnet restore
```
4. Run the application
```bash
dotnet run --project SeawaveAPI
```

### For HTTPS use
In the root folder, where SeawaveAPI.slnx sits create a folder called "certs" and place your certificate and server private key there. Name them dev-cert.crt and dev-cert.key respectively.

