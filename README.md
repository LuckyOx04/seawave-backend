# Seawave Web API

This is a web API that acts as a backend for a music streaming platform. It registrates users, keeps track of music files, uploads and playlists, querries searches and streams a music file to the user.

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- Dapper
- MariaDB
- BCrypt
- MailKit

## How to Run Locally (For Developers)

### Prerequisites
- .NET 10 SDK or Later
- MariaDB Server

### Installation & Run
1. Clone the repository:
```bash
git clone [https://github.com/yourusername/your-repo.git](https://github.com/yourusername/your-repo.git)
```
2. Open appsettings.Development.json and place the values for your case. Do the same with appsettings.json.

3. Navigate to the project directory where SeawaveAPI.slnx file is located and restore dependencies:
```bash
dotnet restore
```
4. Run the application with https or http profile
```bash
dotnet run --project SeawaveAPI --launch-profile [http;https]
```

### For HTTPS use
In the root folder, where SeawaveAPI.slnx sits create a folder called "certs" and place your certificate and server private key there. Name them dev-cert.crt and dev-cert.key respectively.

## How to Run Locally (For Normal Users)

### Prerequisites
- MariaDB Server

1. Download the **seawave-api.tar.gz** file

2. Extract the file
```bash
tar -xzvf seawave-api.tar.gz
```
3. Go in the **seawave-api** folder
```bash
cd seawave-api
```
4. Confugure **appsettings.json** to fit your needs with a text editor of your choice

5. Run the program
```bash
dotnet SeawaveAPI.dll
```

## How to Run via Docker Compose

### Prerequisites
- Docker
- Docker Compose

1. Download the **seawave-api-docker-compose.tar.gz** file

2. Extract the file
```bash
tar -xzvf seawave-api-docker-compose.tar.gz
```
3. Go in the **seawave-api-docker-compose** folder
```bash
cd seawave-api-docker-compose
```
4. If you intend to use HTTPS in the **certs** folder place your certificate and private key files

5. Rename **.env.template** to **.env**
```bash
mv .env.template .env
```
6. Edit **.env** to fit your needs with a text editor of your choice

7. Make the container with docker compose
```bash
docker compose up -d
```
8. Now, your container should be up and running on *port 8080* for HTTP or *port 8081* for HTTPS
