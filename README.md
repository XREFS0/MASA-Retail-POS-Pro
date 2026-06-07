# MASA Retail POS Pro

MASA Retail POS Pro is a comprehensive, high-performance Point of Sale (POS) and inventory management system designed for modern retail environments. Built with the latest .NET framework, it provides a robust, offline-first desktop solution that streamlines daily operations, enhances inventory tracking, and provides actionable business insights.

## Core Features

- Point of Sale (POS): A fast, responsive, and intuitive sales interface. Supports barcode scanning, quick product lookups, dynamic cart management, and seamless checkout processes.
- Inventory Management: Full control over products, categories, stock levels, and pricing. Features include low stock alerts and real-time inventory adjustments.
- Customer & Supplier Management: Maintain detailed profiles and transaction histories for both customers and suppliers.
- Advanced Dashboard & Analytics: Real-time visual data analysis, including multi-axis sales charts, top-selling product metrics, and overall profit calculations.
- Invoice Generation: Automated, professional PDF invoice generation using QuestPDF, fully supporting Arabic typography and structured data tables.
- Role-Based Access Control (RBAC): Secure multi-user environment with dedicated roles (Admin, Manager, Cashier, Warehouse) to restrict access to sensitive configurations and financial data.
- System Management & Data Reset: Comprehensive backup and restore functionality, along with an administrative factory reset utility to safely purge test data and reset database sequences.
- Audit Logging: Continuous background logging of critical system events and user actions to ensure accountability.

## Technology Stack

The application is engineered using modern Microsoft technologies and community-driven libraries:

- Framework: .NET 8.0 (WPF)
- Language: C# 12
- Architecture: MVVM (Model-View-ViewModel) utilizing CommunityToolkit.Mvvm
- Database: SQLite with Entity Framework Core (Code-First approach)
- User Interface: MaterialDesignThemes for a sleek, dark-mode native experience
- Charting: LiveChartsCore.SkiaSharpView.WPF for highly interactive data visualization
- Document Generation: QuestPDF for high-performance PDF invoice rendering

## Getting Started

### Prerequisites
- Windows 10 or Windows 11
- .NET 8.0 Desktop Runtime (for deployment)
- .NET 8.0 SDK (for building from source)

### Installation & Execution
1. Clone or download the repository to your local machine.
2. Open a terminal or command prompt in the root project directory.
3. Restore the required NuGet packages:
   dotnet restore
4. Build the application:
   dotnet build -c Release
5. Run the application:
   dotnet run

Alternatively, you can open the MASA.RetailPOS.App.csproj file in Visual Studio 2022 and press F5 to compile and launch.

## Architecture Highlights

The software follows a strict separation of concerns through the MVVM pattern. Dependency Injection (DI) is utilized across the application to provide database contexts, logging services, and backup utilities seamlessly to ViewModels. The database schema is managed via Entity Framework Core migrations, allowing for automated structure updates on application startup.

## License

Copyright (c) 2026 MASA Systems. All rights reserved.
