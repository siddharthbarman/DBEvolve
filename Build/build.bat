@echo off
rem ## Main build file for DBEvolve ##
rem Path to MSBUILD.EXE must be added to env PATH.
rem E.g. C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin

SET DEV_ENV_CMD=C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvarsall.bat

if "%DEV_ENV_CMD%"=="" (
    echo DEV_ENV_CMD variable is not set. It must point to VsDevCmd.bat.
    echo e.g. SET DEV_ENV_CMD="C:\Program Files\Microsoft Visual Studio\18\Community\Common7\Tools\vcvarsall.bat"
    SET MESSAGE=Build aborted
    goto END
)

if "%1"=="" (
	echo No build configuration either Release nor Debug has been specified.
	SET MESSAGE="Nothing was built."
	goto END
)
else (	
	echo Starting build with %1
)

SET CONFIG=%1

rem ## Setup paths ##
SET ZIP_EXE="C:\Utilities\zip.exe"

rem ## Clean existing binaries ##
echo Cleaning solution

if exist "..\Source\DBEvolveLib\bin\%CONFIG%" (
	RMDIR /S /Q "..\Source\DBEvolveLib\bin\%CONFIG%"
)
if exist "..\Source\DBEvolve\obj\%CONFIG%" (
    RMDIR /S /Q "..\Source\DBEvolve\obj\%CONFIG%"
)

if exist "..\Source\DBEvolveLib.MySql\bin\%CONFIG%" (
	RMDIR /S /Q "..\Source\DBEvolveLib.MySql\bin\%CONFIG%"
)
if exist "..\Source\DBEvolveLib.MySql\obj\%CONFIG%" (
    RMDIR /S /Q "..\Source\DBEvolveLib.MySql\obj\%CONFIG%"
)

if exist "..\Source\DBEvolveLib.Postgres\bin\%CONFIG%" (
	RMDIR /S /Q "..\Source\DBEvolveLib.Postgres\bin\%CONFIG%"
)
if exist "..\Source\DBEvolveLib.Postgres\obj\%CONFIG%" (
    RMDIR /S /Q "..\Source\DBEvolveLib.Postgres\obj\%CONFIG%"
)

if exist "..\Source\DBEvolveLib.Postgres\bin\%CONFIG%" (
	RMDIR /S /Q "..\Source\DBEvolveLib.Postgres\bin\%CONFIG%"
)
if exist "..\Source\DBEvolveLib.SqlServer\obj\%CONFIG%" (
    RMDIR /S /Q "..\Source\DBEvolveLib.SqlServer\obj\%CONFIG%"
)

if exist "..\Source\DBEvolver\bin\%CONFIG%" (
	RMDIR /S /Q "..\Source\DBEvolver\bin\%CONFIG%"
)
if exist "..\Source\DBEvolver\obj\%CONFIG%" (
    RMDIR /S /Q "..\Source\DBEvolver\obj\%CONFIG%"
)

echo Cleaning completed.

call cmd /s /c ""%DEV_ENV_CMD%" && set"

rem ## Build sources ##
echo Starting build...
msbuild.exe ..\Source\DBEvolve.sln /t:Build /p:Configuration="%1" /p:Platform="Any CPU" /p:CheckEolTargetFramework=false /fileLogger

rem # %HHC_PATH% help\help.hhp

rem ## Start packaging ##
rem #if not exist "package" (
rem #	mkdir "package"
rem #)
rem # if exist "package\winid3.zip" (
rem # 	del /F /Q package\winid3.zip
rem # )

rem # copy source\Out\x64\%1\winid3.exe package\
rem # copy help\help.chm package\
rem # copy help\readme.txt package\
rem # %ZIP_EXE% -9 package\winid3.zip package\winid3.exe package\Readme.txt package\help.chm
SET MESSAGE=Build complete.

:END
echo 
echo %MESSAGE%

