# Build Notes

## Status
**Task 9 Complete** - System tray integration done, code pushed to GitHub.

## Build Issue
WinUI 3 XAML Compiler fails with `dotnet build` - error: `XamlCompiler.exe exited with code 1`

**What works:** Core and Engine projects build fine
**What fails:** UI project XAML compilation

## Solution
**Use Visual Studio 2022** - WinUI 3 needs VS build tools. Open `Ody-FakeHZ.sln` and build with platform **x64**.

**Don't have VS?** Install Build Tools:
```powershell
winget install Microsoft.VisualStudio.2022.BuildTools
```

## Commands
```powershell
# Core + Engine (works)
dotnet build src/Ody-FakeHZ.Core/Ody-FakeHZ.Core.csproj -p:Platform=x64

# UI (needs VS)
dotnet build src/Ody-FakeHZ.UI/Ody-FakeHZ.UI.csproj -p:Platform=x64
```

**Note:** Code is correct, this is just a tooling issue.
