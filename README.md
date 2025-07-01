# BusBuddy WPF Application

A comprehensive WPF application for school transportation management, built with Syncfusion WPF controls and Entity Framework Core.

## 🚌 Features
- **Route Management**: View and manage bus routes with Syncfusion SfDataGrid
- **Bus Fleet Management**: Track bus information, capacity, and status
- **Driver Management**: Manage driver records and assignments
- **Modern UI**: Syncfusion MaterialDark theme with professional styling
- **Database Integration**: Entity Framework Core with SQL Server Express
- **MVVM Architecture**: Clean separation with ViewModels and RelayCommand

## 🛠️ Technology Stack
- **.NET 8.0** (WPF Framework)
- **Syncfusion WPF Controls** (SfDataGrid, MaterialDark Theme)
- **Entity Framework Core** with SQL Server Express
- **MVVM Pattern** with RelayCommand implementation

## 🚀 Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server Express (LocalDB)
- Syncfusion Community License Key

### Installation
1. **Clone the repository**:
   ```pwsh
   git clone https://github.com/Bigessfour/BusBuddy-WPF.git
   cd "BusBuddy-WPF"
   ```

2. **Set up Syncfusion License**:
   ```pwsh
   $env:SYNCFUSION_LICENSE_KEY = "YOUR_LICENSE_KEY_HERE"
   ```

3. **Configure Database**:
   - Update connection string in `appsettings.json` if needed
   - Default: `Server=localhost;Database=busbuddy;Trusted_Connection=True;MultipleActiveResultSets=True;`

4. **Build and Run**:
   ```pwsh
   dotnet restore
   dotnet build "BusBuddy Blazer.sln"
   dotnet run --project BusBuddy.csproj
   ```

## 📁 Project Structure
```
├── Data/                    # Entity Framework DbContext
├── Models/                  # Entity models (Bus, Driver, Route, etc.)
├── ViewModels/             # MVVM ViewModels with RelayCommand
├── Views/                  # UserControl views for management
├── DashboardWindow.xaml    # Main application window
├── appsettings.json        # Configuration and connection strings
└── App.xaml.cs            # Application startup and Syncfusion licensing
```

## 🎯 Development Guidelines
- **Syncfusion Usage**: Only use [official Syncfusion WPF documentation](https://help.syncfusion.com/wpf)
- **Code Standards**: Follow BusBuddy guidelines for formatting and file management
- **Build Tools**: Use PowerShell scripts (`build.ps1`, `test.ps1`) for automation
- **Git Hygiene**: Clean repository with proper `.gitignore` for .NET projects

## 📚 Key Components
- **SfDataGrid**: Professional data grids for Routes, Buses, and Drivers
- **TabControl**: Multi-module interface organization
- **MaterialDark Theme**: Consistent modern styling throughout
- **Entity Framework**: Database-first approach with proper connection management

## 🔧 Build Tasks
- **Build**: `dotnet build "BusBuddy Blazer.sln"`
- **Run**: `dotnet run --project BusBuddy.csproj`
- **Clean**: `dotnet clean "BusBuddy Blazer.sln"`

## 📄 License
This project uses Syncfusion Community License. Ensure you have a valid license key set as an environment variable before running the application.
