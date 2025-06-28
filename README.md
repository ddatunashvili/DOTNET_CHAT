
# ChatApp

A real-time chat application built with ASP.NET Core, SignalR, and SQLite.  
It includes user authentication and authorization using ASP.NET Core Identity with a custom `ApplicationUser` model.

---

## Features

- User registration and login with ASP.NET Core Identity  
- Real-time messaging with SignalR hubs  
- Message persistence in SQLite database  
- Seen/unseen message tracking  
- Dark-themed UI with Bootstrap 5  
- Clean architecture separating Identity, Data, Models, and Hubs  

---

## Technologies Used

- ASP.NET 8.0.408 (or your version)  
- Entity Framework Core with SQLite  
- ASP.NET Core Identity  
- SignalR for real-time WebSocket communication  
- Bootstrap 5 for UI styling  

---

## Getting Started

### Prerequisites

- [.NET SDK 8.0 or later](https://dotnet.microsoft.com/download)  
- A code editor such as [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)  

### Setup

1. Clone the repository  

```bash
   git clone https://github.com/ddatunashvili/ChatApp.git
   cd ChatApp
```

2. Restore NuGet packages

   ```bash
   dotnet restore
   ```

3. Create the database and apply migrations

   ```bash
   dotnet ef database update
   ```

4. Run the application

   ```bash
   dotnet run
   ```

5. Open your browser and navigate to `http://localhost:5000` (or the URL shown in the terminal)

---

## Project Structure
```
ChatApp/
├── Areas/
│   └── Identity/           (Razor Pages for login, registration, user management)
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Migrations/         (EF Core migrations)
├── Hubs/                   (SignalR hubs for real-time chat)
├── Models/                 (ApplicationUser, Message, and other data models)
├── wwwroot/                (Static assets: CSS, JS, images)
├── appsettings.json        (Configuration file)
├── Program.cs              (App startup and service setup)
└── ChatApp.csproj          (Project definition file)

```
## Customization

* To change the database file location, update your connection string in `appsettings.json`.
* You can modify the SignalR hub logic in `ChatHub.cs` to add new real-time features.

---

## Troubleshooting

* If migrations fail due to existing migrations, consider deleting the `app.db` SQLite file and the Migrations folder, then recreate migrations.
* Ensure connection strings are correct and SQLite dependencies are installed.

---


## Contact

For questions or feedback, please contact [davitidatunashvili98@gmail.com](mailto:davitidatunashvili98@gmail.com).

---

*Happy coding!* 🚀

