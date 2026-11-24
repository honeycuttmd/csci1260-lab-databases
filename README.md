# CSCI 1260 Lab: Databases

**Name:** Madison Honeycutt  
**Section:** 002  
**Language:** C# with Avalonia

## What I built

This lab uses C#, Avalonia, and SQLite to create a Lost and Found application. Users can add, search for, and claim lost items, while the database saves the information so it remains available after the application closes.

## How to run

To open the Lost and Found window, use:

```
dotnet run
```

To build the project and run the automated lab check in Visual Studio on Windows, use:

```
dotnet build
dotnet .\bin\Debug\net10.0\DatabasesLab.dll check
```

The starter project uses `WinExe`, which prevented `dotnet run -- check` from displaying console output in my Visual Studio's Windows terminal. Running the compiled DLL through the .NET host displays the results instead. Building first ensures the check uses the latest changes.

On other operating systems, the standard check command is:

```
dotnet run -- check
```

The automated check runs all three parts and prints their results to the console.

## One thing that surprised me

It was surprising to see that the database saves information between application sessions without needing a separate database server. It was pretty cool to close the window, open it again, and see that everything was still there.
