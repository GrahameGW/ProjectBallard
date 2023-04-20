# ProjectBallard

## Setup

1. Pull the repo
2. Get the latest .NET version of Godot 4 - https://godotengine.org/download/windows/
3. Open the launcher and import the project folder
4. Select the project and press "Edit

If you want to use an external editor for the C# scripts (e.g., Visual Studio), open the project and go **Editor** -> **Editor Settings** -> **Dotnet** -> **Editor** to select the external editor

Debugging in Visual Studio requires creating a Debug Profile with the following parameters:
- Executable: path to Godot Executable
- Working directory: project folder
 - If you want to see the C++/GD Script interopt code, check "Native Code Debugging"
