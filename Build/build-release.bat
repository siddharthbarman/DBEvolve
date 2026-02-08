echo off

if "%1"=="" (
	echo Package version not specified.
	SET MESSAGE="Nothing was built."
	goto END
)

rem ## Set package version ##
SET PACKAGE_VERSION=%1
spv ..\Source\DBEvolveLib\DBEvolveLib.csproj "%PACKAGE_VERSION%"
spv ..\Source\DBEvolveLib.MySql\DBEvolveLib.MySql.csproj "%PACKAGE_VERSION%"
spv ..\Source\DBEvolveLib.Postgres\DBEvolveLib.Postgres.csproj "%PACKAGE_VERSION%"
spv ..\Source\DBEvolveLib.SqlServer\DBEvolveLib.SqlServer.csproj "%PACKAGE_VERSION%"
spv ..\Source\DBEvolver\DBEvolver.csproj "%PACKAGE_VERSION%"

rem ## Set file & assembly version ##
pv ..\Source\DBEvolveLib\DBEvolveLib.csproj -v %PACKAGE_VERSION%.100
pv ..\Source\DBEvolveLib.MySql\DBEvolveLib.MySql.csproj -v %PACKAGE_VERSION%.100
pv ..\Source\DBEvolveLib.Postgres\DBEvolveLib.Postgres.csproj -v %PACKAGE_VERSION%.100
pv ..\Source\DBEvolveLib.SqlServer\DBEvolveLib.SqlServer.csproj -v %PACKAGE_VERSION%.100
pv ..\Source\DBEvolver\DBEvolver.csproj -v %PACKAGE_VERSION%.100

rem ## Start build ##
call build.bat "Release"
echo "****************************************************************"

rem ## Copy release binaries ##
echo  ..\Releases\%PACKAGE_VERSION%
md ..\Releases\%PACKAGE_VERSION%

set PROJECT_NAME=DBEvolveLib.MySql
set ZIP_FILENAME=%PROJECT_NAME%.zip
cd ..\Source\%PROJECT_NAME%\bin\Release
zip %ZIP_FILENAME% -r *.*
move %ZIP_FILENAME% ..\..\..\..\Releases\%PACKAGE_VERSION%
cd ..\..\..\..\Build
echo MySql binaries copied to release.

set PROJECT_NAME=DBEvolveLib.Postgres
set ZIP_FILENAME=%PROJECT_NAME%.zip
cd ..\Source\%PROJECT_NAME%\bin\Release
zip %ZIP_FILENAME% -r *.*
move %ZIP_FILENAME% ..\..\..\..\Releases\%PACKAGE_VERSION%
cd ..\..\..\..\Build
echo Postgres binaries copied to release.

set PROJECT_NAME=DBEvolveLib.SqlServer
set ZIP_FILENAME=%PROJECT_NAME%.zip
cd ..\Source\%PROJECT_NAME%\bin\Release
zip %ZIP_FILENAME% -r *.*
move %ZIP_FILENAME% ..\..\..\..\Releases\%PACKAGE_VERSION%
cd ..\..\..\..\Build
echo SqlServer binaries copied to release.

set PROJECT_NAME=DBEvolver
set ZIP_FILENAME=%PROJECT_NAME%.zip
cd ..\Source\%PROJECT_NAME%\bin\Release
zip %ZIP_FILENAME% -r *.*
move %ZIP_FILENAME% ..\..\..\..\Releases\%PACKAGE_VERSION%
cd ..\..\..\..\Build
echo DBEvolver binaries copied to release.

SET MESSAGE=Release created successfully.

:END
echo 
echo %MESSAGE%
