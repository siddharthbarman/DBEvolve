# Testing packages locally

## Clear local cache
```
dotnet nuget locals all --clear
```

## Start the databases
```
docker run --name postgres -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres:14
docker run --name mysql -e MYSQL_ROOT_PASSWORD=password123 -p 3306:3306 -d mysql:8.0.45
```

## Create local package sources
```
dotnet nuget add source "F:\Work\Github\DBEvolve\Releases\1.3.8\DBEvolveLib.MySql" -n LocalSourceMySql
dotnet nuget add source "F:\Work\Github\DBEvolve\Releases\1.3.0\DBEvolveLib.Postgres" -n LocalSourcePostgres
dotnet nuget add source "F:\Work\Github\DBEvolve\Releases\1.3.0\DBEvolveLib.SqlServer" -n LocalSourceSqlServer
```

## Delete local sources
dotnet nuget remove  source LocalSourcePostgres
dotnet nuget remove  source LocalSourceMySql
dotnet nuget remove  source LocalSourceSqlServer
dotnet nuget locals all --clear

# Stop databases
```
docker stop postgres
docker rm postgres
docker image rm postgres:14

docker stop mysql
docker rm mysql
docker image rm mysql:8.0.45
```