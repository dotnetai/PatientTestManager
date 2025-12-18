# Patient Test Manager

### Setup Instructions (README equivalent)
1. **Prerequisites**:
   - Visual Studio 2022 or later with .NET desktop development workload.
   - SQL Server (LocalDB or full instance). If using LocalDB, it's installed with Visual Studio.
   - NuGet packages: Install `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Tools` via NuGet Package Manager.

2. **Project Creation**:
   - Create a new WPF App (.NET Framework or .NET 8+; I used .NET 8 for this example).
   - Add the models, DbContext, and update MainWindow.xaml / .cs as below.
   - Update the connection string in `App.config` or `appsettings.json` (for .NET Core/8, use appsettings.json, but since WPF can be .NET Framework, I'll assume App.config for simplicity. Adjust if needed).

3. **Database Setup**:
   - In Package Manager Console: Run `Add-Migration InitialCreate` then `Update-Database` to create the database via code-first.

4. **Running**:
   - Build and run the application.
   - The UI has a DataGrid for patients on the left. Select a patient to show tests on the right.
   - Buttons for Add/Edit/Delete patients and tests.
   - Report section at the bottom: Enter from/to dates and click "Generate Report" to show in a DataGrid.

5. **Connection String**:
   - Example for LocalDB: `"Server=(localdb)\\mssqllocaldb;Database=PatientTestDb;Trusted_Connection=True;"`
   - Place in App.config under `<connectionStrings>` or adjust in DbContext.
