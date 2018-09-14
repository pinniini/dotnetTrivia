# dotnetTrivia
Command line trivia game made with .NET Core.

Uses [Open Trivia Database](https://opentdb.com/) as the source for the questions.

## Building
.NET Core SDK needs to be installed. Instructions on how to install it can be found from the following link [.NET Tutorial](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial)

To build the game run the following command in the project folder:
```
dotnet build
```

To publish the game as a self-contained deployment for linux run the following command:
```
dotnet publish -c Release -r linux-x64
```

To publish for Windows replace the `linux-x64` with `win10-x64` and for OSX `osx-x64`.

## Running
To run the game run the following command in the project folder:
```
dotnet run
```