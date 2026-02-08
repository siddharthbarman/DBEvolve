echo off

rem ## Incrementing build number ##
pv  ..\Source\DBEvolveLib\DBEvolveLib.csproj
pv  ..\Source\DBEvolver\DBEvolver.csproj

rem ## Start build ##
build.bat "Debug"

