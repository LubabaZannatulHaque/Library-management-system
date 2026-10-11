# Library Management System 📚

A fixed C# .NET 4.8 desktop application for managing library books, members, borrow records, and penalties. Built with Windows Forms and Guna.UI2.

---

## Screenshots

### Manage Books
![Manage Books](screenshots/manage_books.png)

Add, update, delete, and search books by title, author, publisher, or category.

### Borrow Records
![Borrow Records](screenshots/borrow_records.png)

Track borrowed books, penalties, return dates, and timer status for all users.

### Manage Users
![Manage Users](screenshots/manage_users.png)

Register new members, set roles (Admin / Student), and manage user accounts.

### Timer & Penalty Alert
![Timer Penalty](screenshots/timer_penalty.png)

When the borrow timer expires, a penalty is applied automatically (e.g., 20.00 Taka for 2 books).

### Student Dashboard — Books & Borrowed
![Student Dashboard Books](screenshots/student_dashboard_1.png)

Students can view all available books, search by title or author, and see borrowed records.

### Student Dashboard — Timer Expired
![Student Dashboard Timer](screenshots/student_dashboard_2.png)

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
