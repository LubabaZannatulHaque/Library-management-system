# Library Management System 📚

A fixed C# .NET 4.8 desktop application for managing library books, members, borrow records, and penalties. Built with Windows Forms and Guna.UI2.

---

## Screenshots

### Manage Books
<img width="911" height="618" alt="Screenshot 2026-10-10 172118" src="https://github.com/user-attachments/assets/3d87477c-b4ae-40b6-9965-51b9a8c96f54" />

Add, update, delete, and search books by title, author, publisher, or category.

### Borrow Records
<img width="1031" height="663" alt="Screenshot 2026-10-10 172137" src="https://github.com/user-attachments/assets/77dfff1d-716d-4d6e-a8a6-2ff31446e366" />


Track borrowed books, penalties, return dates, and timer status for all users.

### Manage Users
<img width="1033" height="661" alt="Screenshot 2026-10-10 172200" src="https://github.com/user-attachments/assets/1e8ceb8e-38c1-4e9d-87e6-66ef46ec91a6" />


Register new members, set roles (Admin / Student), and manage user accounts.

### Timer & Penalty Alert
<img width="428" height="165" alt="Screenshot 2026-10-10 172230" src="https://github.com/user-attachments/assets/0a3b6216-a94e-4ca5-8c9c-fede8b0fc171" />


When the borrow timer expires, a penalty is applied automatically (e.g., 20.00 Taka for 2 books).

### Student Dashboard — Books & Borrowed
<img width="1236" height="693" alt="Screenshot 2026-10-10 172303" src="https://github.com/user-attachments/assets/94f992cb-721f-4c84-85b1-897f4dc1c18b" />


Students can view all available books, search by title or author, and see borrowed records.

### Student Dashboard — Timer Expired
<img width="1249" height="694" alt="Screenshot 2026-10-10 172314" src="https://github.com/user-attachments/assets/f24a07b5-56f8-41b1-bc3b-9004cedb2a1f" />


Borrowed books show status, penalties applied, and timer status directly in the dashboard.

---

## Features

- Book catalog management (add, edit, delete, search)
- User registration (Admin / Student roles)
- Borrow and return tracking with timer
- Automatic penalty application on timer expiration
- SQL Server database with parameterized queries
- Clean Guna.UI2 interface

---

## Technology

- **Language:** C# (.NET Framework 4.8)
- **UI:** Windows Forms + Guna.UI2
- **Database:** SQL Server
- **Storage:** `library.db` (SQLite) available in `simple_lms/` for quick testing

---

## Quick Start

1. Open `LibraryManagementSystem.sln` in Visual Studio 2019/2022.
2. Restore packages: `nuget restore` or use Package Manager Console.
3. Run SQL script: `SQL_Script.sql` (creates database + tables + sample data).
4. Build: `Ctrl+Shift+B`
5. Run: `Ctrl+F5`

---

## Fixes Applied (Compared to Original)

- `App.config`: Added `LibraryDB` connection string (localhost based).
- `DataAccess.cs`: Replaced hardcoded `DESKTOP-S3IFP7G` server with configurable string.
- `LibraryManagementSystem.csproj`: Added `System.Configuration` reference.
- `ManageBooks.cs` / `StudentDashboard.cs` / `BorrowRecords.cs`: SQL injection fixed (parameterized queries).
- `packages.config`: Guna.UI2 package reference included.

---

## Python Simple Version (No Visual Studio Needed)

For quick testing or learning:

```bash
python simple_lms/library_simple.py
```

Creates `library.db` (SQLite) automatically.

---

## Requirements

- Windows OS
- Visual Studio 2019/2022 (for C# version)
- SQL Server (Express / Developer / Full)
- .NET Framework 4.8 Developer Pack
- Python 3 (optional, for simple version)

---

## Repository Info

- **Name:** Library-management-system
- **Status:** Fixed and ready for build/deployment.
- **Files included:** Source code, SQL script, screenshots, README, Python alternative.
